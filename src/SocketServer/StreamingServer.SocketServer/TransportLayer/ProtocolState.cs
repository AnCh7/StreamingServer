namespace StreamingServer.SocketServer.TransportLayer
{
	public class ProtocolState
	{
		private readonly RequestBuilder _requestBuilder = new RequestBuilder();
		private readonly ResponseBuilder _responseBuilder = new ResponseBuilder();

		public RequestBuilder RequestBuilder
		{
			get { return _requestBuilder; }
		}

		public ResponseBuilder ResponseBuilder
		{
			get { return _responseBuilder; }
		}

		public void ResetRequest()
		{
			_requestBuilder.Reset();
		}

		public void ResetResponse()
		{
			_responseBuilder.Reset();
		}
	}
}
