#region Usings

using System;
using System.Diagnostics;
using System.Net.Sockets;
using StreamingServer.SocketServer.Interfaces;

#endregion

namespace StreamingServer.SocketServer.TransportLayer
{
	public class Protocol : IProtocol
	{
		private readonly IProtocolHandler _handler;

		/// <exception cref="ArgumentNullException">The value of '' cannot be null. </exception>
		public Protocol(IProtocolHandler handler)
		{
			if (handler == null)
			{
				throw new ArgumentNullException();
			}

			_handler = handler;
		}

		public void InitState(SocketAsyncEventArgs args)
		{
			args.UserToken = new ProtocolState();
		}

		public bool ProcessReceivedFrame(SocketAsyncEventArgs args)
		{
			var state = args.UserToken as ProtocolState;
			Debug.Assert(state != null, "state != null");

			var segment = new ArraySegment<byte>(args.Buffer, args.Offset, args.BytesTransferred);
			var completed = state.RequestBuilder.Process(segment);

			if (completed)
			{
				_handler.HandleRequest(state.RequestBuilder.Request);
			}

			return completed;
		}

		public void PrepareFrameToSend(SocketAsyncEventArgs args, int bufferSize)
		{
			var state = args.UserToken as ProtocolState;
			Debug.Assert(state != null, "state != null");

			_handler.HandleResponse(state.RequestBuilder.Request, state.ResponseBuilder.Response);

			var segment = new ArraySegment<byte>(args.Buffer, args.Offset, bufferSize);
			var bytesWritten = state.ResponseBuilder.Process(segment);
			args.SetBuffer(args.Offset, bytesWritten);
		}

		public bool IsSendCompleted(SocketAsyncEventArgs args)
		{
			var state = args.UserToken as ProtocolState;
			Debug.Assert(state != null, "state != null");

			var completed = state.ResponseBuilder.Completed;
			if (completed)
			{
				state.ResetResponse();
			}

			return false;
		}

		public bool IsCloseRequired(SocketAsyncEventArgs args)
		{
			// Always false because of streaming architecture
			return false;
		}

		public void Dispose(SocketAsyncEventArgs args)
		{
			Dispose(true, args);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing, SocketAsyncEventArgs args)
		{
			if (disposing)
			{
				var state = args.UserToken as ProtocolState;
				Debug.Assert(state != null, "state != null");

				_handler.Dispose(state.RequestBuilder.Request);
			}
		}
	}
}