#region Usings

using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

using StreamingServer.SocketServer.Interfaces;
using StreamingServer.SocketServer.Models;
using StreamingServer.SocketServer.Monitor;

#endregion

namespace StreamingServer.SocketServer
{
	public class SocketServer
	{
		private readonly IProtocol _protocol;
		private readonly ServerMonitor _monitor;

		private Socket _listenSocket;
		private EndPoint _localEndPoint;

		private readonly ConcurrentStack<SocketAsyncEventArgs> _acceptArgsPool;
		private readonly ConcurrentStack<SocketAsyncEventArgs> _clientArgsPool;

		private readonly ServerSettings _settings;

		public SocketServer(ServerSettings settings, ServerMonitor monitor, IProtocol protocol)
		{
			_settings = settings;
			_monitor = monitor;
			_protocol = protocol;

			_acceptArgsPool = new ConcurrentStack<SocketAsyncEventArgs>();
			_clientArgsPool = new ConcurrentStack<SocketAsyncEventArgs>();
		}

		public void Start()
		{
			_localEndPoint = new IPEndPoint(IPAddress.Any, _settings.Port);
			_listenSocket = new Socket(_localEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

			_listenSocket.Bind(_localEndPoint);
			_listenSocket.Listen(_settings.Backlog);

			_monitor.ServerUrl = _listenSocket.LocalEndPoint.ToString();

			StartAccept();
		}

		private void StartAccept()
		{
			// Get a SocketAsyncEventArgs object to accept the connection.
			SocketAsyncEventArgs args;

			// Get it from the pool if there is more than one in the pool.
			// or make a new one.
			if (!_acceptArgsPool.TryPop(out args))
			{
				args = new SocketAsyncEventArgs();
				args.Completed += SocketOperationCompleted;
			}

			// Socket.AcceptAsync begins asynchronous operation to accept the connection.
			var willRaiseEvent = _listenSocket.AcceptAsync(args);

			// Socket.AcceptAsync returns true if the I/O operation is pending, i.e. is working asynchronously.
			if (!willRaiseEvent)
			{
				// The code in this if (!willRaiseEvent) statement only runs
				// when the operation was completed synchronously. It is needed because
				// when Socket.AcceptAsync returns false,
				// it does NOT raise the SocketAsyncEventArgs.Completed event.
				// And we need to call ProcessAccept and pass it the SAEA object.
				// This is only when a new connection is being accepted.
				Task.Run(() => ProcessAccept(args));
			}

			_monitor.StartAccept();
		}

		/// <summary>
		///     SocketAsyncEventArgs object that will do receive/send.
		/// </summary>
		/// <param name="args"> </param>
		private void ProcessAccept(SocketAsyncEventArgs args)
		{
			_monitor.ProcessAccept();

			// Loop back to post another accept op.
			StartAccept();

			// This is when there was an error with the accept op. 
			if (args.SocketError != SocketError.Success)
			{
				CloseClientSocket(args.AcceptSocket, args);
			}
			else
			{
				// Get a SocketAsyncEventArgs object from the pool of receive/send op //SocketAsyncEventArgs objects
				SocketAsyncEventArgs clientArgs;
				if (!_clientArgsPool.TryPop(out clientArgs))
				{
					clientArgs = new SocketAsyncEventArgs();
					var buffer = new byte[_settings.BufferSize];
					clientArgs.SetBuffer(buffer, 0, buffer.Length);
					clientArgs.Completed += SocketOperationCompleted;
					_protocol.InitState(clientArgs);
				}

				// A new socket was created by the AcceptAsync method. The
				// SocketAsyncEventArgs object which did the accept operation has that
				// socket info in its AcceptSocket property. Now we will give
				// a reference for that socket to the SocketAsyncEventArgs object which will do receive/send.
				clientArgs.AcceptSocket = args.AcceptSocket;

				// We handed off the connection info from the accepting socket to the receiving socket. 
				StartReceive(clientArgs);
			}

			// Clear the socket info from that object, so it will be ready for a new socket when it comes out of the pool.
			args.AcceptSocket = null;
			_acceptArgsPool.Push(args);
		}

		/// <summary>
		///     Post a receive op.
		/// </summary>
		/// <param name="args"> </param>
		private void StartReceive(SocketAsyncEventArgs args)
		{
			// Post async receive operation on the socket.
			var willRaiseEvent = args.AcceptSocket.ReceiveAsync(args);

			// Socket.ReceiveAsync returns false if I/O operation completed synchronously.
			if (!willRaiseEvent)
			{
				// If the op completed synchronously, we need to call ProcessReceive method directly. 
				Task.Run(() => ProcessReceive(args));
			}

			_monitor.StartReceive();
		}

		/// <summary>
		///     If the remote host closed the connection, then the socket is closed.
		///     Otherwise, we process the received data. And if a complete message was received,
		///     then we do some additional processing, to respond to the client.
		/// </summary>
		/// <param name="args"> </param>
		private void ProcessReceive(SocketAsyncEventArgs args)
		{
			_monitor.ProcessReceive();

			// If there was a socket error, close the connection.
			if (args.SocketError != SocketError.Success)
			{
				CloseClientSocket(args.AcceptSocket, args);
				_clientArgsPool.Push(args);
			}
			// If no data was received, close the connection. 
			// This is a situation that shows when the client has finished sending data.
			else if (args.BytesTransferred == 0)
			{
				CloseClientSocket(args.AcceptSocket, args);
				_clientArgsPool.Push(args);
			}
			else
			{
				var lastFrame = _protocol.ProcessReceivedFrame(args);
				if (lastFrame)
				{
					_protocol.PrepareFrameToSend(args, _settings.BufferSize);
					StartSend(args);
				}
				else
				{
					StartReceive(args);
				}
			}
		}

		private void StartSend(SocketAsyncEventArgs args)
		{
			var willRaiseEvent = false;

			if (args.Count > 0)
			{
				_monitor.StartSend();
				willRaiseEvent = args.AcceptSocket.SendAsync(args);
			}

			if (!willRaiseEvent)
			{
				Task.Run(() => ProcessSend(args));
			}
		}

		/// <summary>
		///     This method is called by I/O Completed() when an asynchronous send completes.
		///     If all of the data has been sent, then this method calls StartReceive to start another receive op
		///     on the socket to read any additional  data sent from the client.
		///     If all of the data has NOT been sent, then it calls StartSend to send more data.
		/// </summary>
		/// <param name="args"> </param>
		private void ProcessSend(SocketAsyncEventArgs args)
		{
			if (args.Count > 0)
			{
				_monitor.ProcessSend();
			}

			if (args.SocketError != SocketError.Success)
			{
				CloseClientSocket(args.AcceptSocket, args);
				_clientArgsPool.Push(args);
			}
			else
			{
				var lastFrame = _protocol.IsSendCompleted(args);
				if (lastFrame)
				{
					var closeRequired = _protocol.IsCloseRequired(args);
					if (closeRequired)
					{
						CloseClientSocket(args.AcceptSocket, args);
						_clientArgsPool.Push(args);
					}
					else
					{
						StartReceive(args);
					}
				}
				else
				{
					_protocol.PrepareFrameToSend(args, _settings.BufferSize);
					StartSend(args);
				}
			}
		}

		/// <summary>
		///     This method is called whenever a accept, receive or send operation completes.
		///     Here args represents the SocketAsyncEventArgs object associated with the completed accept or receive or send operation.
		/// </summary>
		/// <param name="sender"> </param>
		/// <param name="args"> </param>
		private void SocketOperationCompleted(object sender, SocketAsyncEventArgs args)
		{
			// Determine which type of operation just completed and call the associated handler
			switch (args.LastOperation)
			{
				case SocketAsyncOperation.Accept:
					ProcessAccept(args);
					break;

				case SocketAsyncOperation.Receive:
					ProcessReceive(args);
					break;

				case SocketAsyncOperation.Send:
					ProcessSend(args);
					break;

				default:
					throw new ArgumentException("The last operation completed on the socket was not a accept/receive/send");
			}
		}

		private void CloseClientSocket(Socket socket, SocketAsyncEventArgs args)
		{
			// Do a shutdown before you close the socket
			try
			{
				socket.Shutdown(SocketShutdown.Both);
				_protocol.Dispose(args);
			}
			catch (Exception)
			{
				// ignored
			}

			// This method closes the socket and releases all resources, both managed and unmanaged.
			socket.Close();

			_monitor.CloseClient();
		}
	}
}
