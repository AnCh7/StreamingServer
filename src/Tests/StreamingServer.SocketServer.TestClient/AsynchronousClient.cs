#region Usings

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using StreamingServer.SocketServer.Models;

#endregion

namespace StreamingServer.SocketServer.TestClient
{
	public class AsynchronousClient
	{
		private readonly string _clientId;

		private readonly ManualResetEvent _connectDone = new ManualResetEvent(false);
		private readonly ManualResetEvent _sendDone = new ManualResetEvent(false);
		private readonly ManualResetEvent _receiveDone = new ManualResetEvent(false);

		public AsynchronousClient(string clientId)
		{
			_clientId = clientId;
		}

		public void StartClient(Request request)
		{
			try
			{
				var ipAddress = IPAddress.Parse(Settings.Url());
				var remoteEndpoint = new IPEndPoint(ipAddress, Settings.Port());
				var client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

				client.BeginConnect(remoteEndpoint, ConnectCallback, client);
				_connectDone.WaitOne();

				// Send test data to the remote device.
				Send(client, request + Environment.NewLine);
				_sendDone.WaitOne();

				// Receive the response from the remote device.
				Receive(client);
				_receiveDone.WaitOne();

				// Release the socket.
				client.Shutdown(SocketShutdown.Both);
				client.Close();
			}
			catch (Exception e)
			{
				Console.WriteLine(_clientId + " - " + e);
			}
		}

		private void ConnectCallback(IAsyncResult ar)
		{
			try
			{
				// Retrieve the socket from the state object.
				var client = (Socket) ar.AsyncState;

				// Complete the connection.
				client.EndConnect(ar);

				Console.WriteLine(_clientId + " - " + "Socket connected to {0}", client.RemoteEndPoint);

				// Signal that the connection has been made.
				_connectDone.Set();
			}
			catch (Exception e)
			{
				Console.WriteLine(_clientId + " - " + e);
			}
		}

		private void Receive(Socket client)
		{
			try
			{
				// Create the state object.
				var state = new StateObject {ClientSocket = client};

				// Begin receiving the data from the remote device.
				client.BeginReceive(state.Buffer, 0, Settings.BufferSize(), 0, ReceiveCallback, state);
			}
			catch (Exception e)
			{
				Console.WriteLine(_clientId + " - " + e);
			}
		}

		private void ReceiveCallback(IAsyncResult ar)
		{
			try
			{
				// Retrieve the state object and the client socket from the asynchronous state object.
				var state = (StateObject) ar.AsyncState;
				var client = state.ClientSocket;

				// Read data from the remote device.
				var bytesRead = client.EndReceive(ar);

				if (bytesRead > 0)
				{
					Console.WriteLine(_clientId + " - " + Encoding.ASCII.GetString(state.Buffer, 0, bytesRead));

					// Get the rest of the data.
					client.BeginReceive(state.Buffer, 0, Settings.BufferSize(), 0, ReceiveCallback, state);
				}
				else
				{
					Console.WriteLine(_clientId + " - " + "All the data has arrived");
					_receiveDone.Set();
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(_clientId + " - " + e);
			}
		}

		private void Send(Socket client, String data)
		{
			// Convert the string data to byte data using ASCII encoding.
			var byteData = Encoding.ASCII.GetBytes(data);

			// Begin sending the data to the remote device.
			client.BeginSend(byteData, 0, byteData.Length, 0, SendCallback, client);
		}

		private void SendCallback(IAsyncResult ar)
		{
			try
			{
				// Retrieve the socket from the state object.
				var client = (Socket) ar.AsyncState;

				// Complete sending the data to the remote device.
				var bytesSent = client.EndSend(ar);
				Console.WriteLine(_clientId + " - " + "Sent {0} bytes to server.", bytesSent);

				// Signal that all bytes have been sent.
				_sendDone.Set();
			}
			catch (Exception e)
			{
				Console.WriteLine(_clientId + " - " + e);
			}
		}
	}
}