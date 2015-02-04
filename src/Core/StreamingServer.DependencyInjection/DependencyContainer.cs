#region Usings

using Autofac;
using StreamingServer.CityIndexLightstreamerClient;
using StreamingServer.CityIndexWebClient;
using StreamingServer.Common.JsonSerializer;
using StreamingServer.Common.Logging;
using StreamingServer.Common.Logging.Targets;
using StreamingServer.Core;
using StreamingServer.SocketServer.Interfaces;
using StreamingServer.SocketServer.TransportLayer;

#endregion

namespace StreamingServer.DependencyInjection
{
	public sealed class DependencyContainer
	{
		private static readonly DependencyContainer _instance = new DependencyContainer();
		private static volatile IContainer _container;

		static DependencyContainer()
		{
			var builder = new ContainerBuilder();

			builder.RegisterType<Logger>()
				   .As<ILogger>()
				   .WithParameter("loggingConfiguration", new TargetWindowsEvents().Initialize()).SingleInstance();

			builder.RegisterType<ServiceStackJsonSerializer>().As<IJsonSerializer>().SingleInstance();

			builder.RegisterType<CityIndexRestClient>().As<ICityIndexRestClient>().SingleInstance();

			builder.RegisterType<StreamingClient>().As<IStreamingClient>().SingleInstance();

			builder.RegisterType<StreamingProtocolHandler>().As<IProtocolHandler>().SingleInstance();

			builder.RegisterType<Protocol>().As<IProtocol>().SingleInstance();

			_container = builder.Build();
		}

		private DependencyContainer()
		{}

		public static IContainer Instance
		{
			get { return _container; }
		}
	}
}
