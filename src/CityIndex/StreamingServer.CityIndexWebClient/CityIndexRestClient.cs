#region Usings

using System;

using RestSharp;

using ServiceStack.Text;

using StreamingServer.CityIndexWebClient.Exceptions;
using StreamingServer.CityIndexWebClient.Models;
using StreamingServer.Common.Logging;
using StreamingServer.Common.Models;

#endregion

namespace StreamingServer.CityIndexWebClient
{
	public interface ICityIndexRestClient : IDisposable
	{
		void Init(string url);

		OperationResult<ApiLogOnResponseDTO> LogOn(ApiLogOnRequestDTO request);
	}

	public class CityIndexRestClient : ICityIndexRestClient
	{
		private ILogger _logger;
		private string _url;

		public CityIndexRestClient(ILogger logger)
		{
			_logger = logger;
		}

		public void Init(string url)
		{
			_url = url;
		}

		/// <summary>
		///     Create a new session. This is how you "log on" to the CIAPI.
		/// </summary>
		/// <param name="request"> </param>
		/// <returns> </returns>
		/// <exception cref="ArgumentNullException"><paramref name="format" /> is null. </exception>
		/// <exception cref="FormatException"><paramref name="format" /> is invalid.-or- The index of a format item is not zero or one. </exception>
		public OperationResult<ApiLogOnResponseDTO> LogOn(ApiLogOnRequestDTO request)
		{
			var url = String.Format("{0}/{1}/", _url, "session");

			try
			{
				var response = Post<ApiLogOnResponseDTO>(url, request);
				return new OperationResult<ApiLogOnResponseDTO>(response);
			}
			catch (RestClientException e)
			{
				return new OperationResult<ApiLogOnResponseDTO>(false, e.Message);
			}
		}

		private T Post<T>(string url, object request)
		{
			IRestRequest restRequest;

			try
			{
				restRequest = new RestRequest(Method.POST);
				restRequest.AddJsonBody(request);
			}
			catch (Exception ex)
			{
				throw new RestClientException(ex);
			}

			return SendRequest<T>(url, restRequest);
		}

		private T SendRequest<T>(string url, IRestRequest request)
		{
			var client = new RestClient(url);
			var response = client.Execute(request);

			if (response.ResponseStatus != ResponseStatus.Completed)
			{
				throw new RestClientException(response.ErrorMessage);
			}
			else
			{
				var errorResponse = response.Content.FromJson<ApiErrorResponseDTO>();
				if (errorResponse.ErrorCode <= 500)
				{
					if (errorResponse.ErrorCode == 403)
					{
						throw new RestClientException("Forbidden");
					}
					if (errorResponse.ErrorCode == 500)
					{
						throw new RestClientException("InternalServerError");
					}
				}
				else
				{
					switch (errorResponse.ErrorCode)
					{
						case 4000:
							throw new RestClientException("InvalidParameterType");

						case 4001:
							throw new RestClientException("ParameterMissing");

						case 4002:
							throw new RestClientException("InvalidParameterValue");

						case 4003:
							throw new RestClientException("InvalidJsonRequest");

						case 4004:
							throw new RestClientException("InvalidJsonRequestCaseFormat");

						case 4005:
						case 4006:
						case 4007:
						case 4008:
						case 4009:
							_logger.Info(errorResponse.ErrorCode + errorResponse.ErrorMessage);
							break;

						case 4010:
							throw new RestClientException("InvalidCredentials");

						case 4011:
							throw new RestClientException("SessionExpired");

						case 5001:
							throw new RestClientException("NoDataAvailable");

						case 5002:
							throw new RestClientException("Throttling");

						default:
							_logger.Error("Not supported error - " + errorResponse.ErrorCode + errorResponse.ErrorMessage);
							break;
					}
				}

				var result = response.Content.FromJson<T>();
				return result;
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				_logger = null;
				_url = string.Empty;
			}
		}
	}
}
