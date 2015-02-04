#region Usings

using System.Net.Sockets;

#endregion

namespace StreamingServer.SocketServer.TestClient
{
	public class StateObject
	{
		public Socket ClientSocket;

		public readonly byte[] Buffer = new byte[Settings.BufferSize()];
	}
}
