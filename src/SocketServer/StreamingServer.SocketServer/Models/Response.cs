#region Usings

using System.IO;

#endregion

namespace StreamingServer.SocketServer.Models
{
	public class Response
	{
		public Stream Content { get; set; }
	}
}
