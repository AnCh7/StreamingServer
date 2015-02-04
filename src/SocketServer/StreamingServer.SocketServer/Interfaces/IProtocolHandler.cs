#region Usings

using StreamingServer.SocketServer.Models;

#endregion

namespace StreamingServer.SocketServer.Interfaces
{
	public interface IProtocolHandler
	{
		void HandleRequest(Request request);

		void HandleResponse(Request request, Response response);

		void Dispose(Request request);
	}
}