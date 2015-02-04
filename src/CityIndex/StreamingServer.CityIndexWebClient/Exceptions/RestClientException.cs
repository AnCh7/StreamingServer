#region Usings

using System;

#endregion

namespace StreamingServer.CityIndexWebClient.Exceptions
{
	[Serializable]
	public class RestClientException : Exception
	{
		public RestClientException(string message) : base(message)
		{
		}

		public RestClientException(string message, Exception exception)
			: base(message, exception)
		{
		}

		public RestClientException(Exception inner)
			: base("CityIndexWebClient Exception", inner)
		{
		}
	}
}