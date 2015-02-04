namespace StreamingServer.SocketServer.Models
{
	public class ServerSettings
	{
		public int Port { get; set; }

		public int Backlog { get; set; }

		public int BufferSize { get; set; }
	}
}
