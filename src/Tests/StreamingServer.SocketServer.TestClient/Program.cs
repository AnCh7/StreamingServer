#region Usings

using System;
using System.Configuration;
using System.Threading.Tasks;
using Autofac;
using StreamingServer.CityIndexWebClient;
using StreamingServer.CityIndexWebClient.Models;
using StreamingServer.DependencyInjection;
using StreamingServer.SocketServer.Models;

#endregion

namespace StreamingServer.SocketServer.TestClient
{
	internal class Program
	{
		private static ICityIndexRestClient _restClient;

		private static void Main(string[] args)
		{
			try
			{
				_restClient = DependencyContainer.Instance.Resolve<ICityIndexRestClient>();
				_restClient.Init(ConfigurationManager.AppSettings["RestUrl"]);

				Run("DM241228", "password", "154289.154290.154291.154292.154293.154294.154296.154297.154298.154299", "11111111111111111");
				Run("DM626102", "DGDGDGDG", "99498.99500.99502.99504.99508.99510.99524.99558.154286.154287", "22222222222222222");

				Console.ReadKey();
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}
		}

		private static void Run(string userName, string password, string prices, string clientId)
		{
			const string appKey = "Test";
			const string appVersion = "1.0.0.0";

			var response = _restClient.LogOn(new ApiLogOnRequestDTO(userName, password, appKey, appVersion));
			if (response.Success)
			{
				Task.Run(() => SubscribeToClientAccountMargin(userName, response.Data.Session, clientId));
				Task.Run(() => SubscribeToTradeMargin(userName, response.Data.Session, clientId));
				Task.Run(() => SubscribeToPrices(userName, response.Data.Session, prices, clientId));
			}
		}

		private static void SubscribeToClientAccountMargin(string userName, string session, string clientId)
		{
			var request = new Request { UserName = userName, SessionToken = session, RequestType = "CLIENTACCOUNTMARGIN" };

			var client = new AsynchronousClient(clientId);
			client.StartClient(request);
		}

		private static void SubscribeToTradeMargin(string userName, string session, string clientId)
		{
			var request = new Request { UserName = userName, SessionToken = session, RequestType = "TRADEMARGIN" };

			var client = new AsynchronousClient(clientId);
			client.StartClient(request);
		}

		private static void SubscribeToPrices(string userName, string session, string prices, string clientId)
		{
			var request = new Request
			{
				UserName = userName,
				SessionToken = session,
				RequestType = "PRICES",
				Parameters = prices
			};

			var client = new AsynchronousClient(clientId);
			client.StartClient(request);
		}
	}
}