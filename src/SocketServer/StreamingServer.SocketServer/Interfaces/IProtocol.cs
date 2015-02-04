#region Usings

using System;
using System.Net.Sockets;

#endregion

namespace StreamingServer.SocketServer.Interfaces
{
	public interface IProtocol
	{
		void InitState(SocketAsyncEventArgs args);

		bool ProcessReceivedFrame(SocketAsyncEventArgs args);

		void PrepareFrameToSend(SocketAsyncEventArgs args, int bufferSize);

		bool IsSendCompleted(SocketAsyncEventArgs args);

		bool IsCloseRequired(SocketAsyncEventArgs args);

		void Dispose(SocketAsyncEventArgs args);
	}
}
