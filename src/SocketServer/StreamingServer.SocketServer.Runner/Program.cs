#region Usings

using System;
using System.Configuration;
using System.Threading;
using Autofac;
using StreamingServer.CityIndexLightstreamerClient;
using StreamingServer.DependencyInjection;
using StreamingServer.SocketServer.Interfaces;
using StreamingServer.SocketServer.Models;
using StreamingServer.SocketServer.Monitor;

#endregion

namespace StreamingServer.SocketServer.Runner
{
	internal class Program
	{
		private static ServerMonitor _monitor;
		private static SocketServer _socketServer;

		private static void Main(string[] args)
		{
			RunServer();
			RunMonitor();

			Console.ReadKey();
		}

		private static void RunServer()
		{
			var streamingUrl = ConfigurationManager.AppSettings["StreamingUrl"];
			DependencyContainer.Instance.Resolve<IStreamingClient>().Init(streamingUrl, false);

			var settings = new ServerSettings
			{
				Port = Settings.Port(),
				Backlog = Settings.Backlog(),
				BufferSize = Settings.BufferSize()
			};

			var protocol = DependencyContainer.Instance.Resolve<IProtocol>();

			_monitor = new ServerMonitor();

			_socketServer = new SocketServer(settings, _monitor, protocol);
			_socketServer.Start();
		}

		private static void RunMonitor()
		{
			Console.WriteLine("Waiting for a connection... Url: " + _monitor.ServerUrl);

			Console.CursorVisible = false;

			while (true)
			{
				Thread.Sleep(1000);

				Console.CursorLeft = 0;
				Console.CursorTop = 1;

				Console.WriteLine("Accepts posted: " + _monitor.AcceptsPosted);
				Console.WriteLine("Accepts completed: " + _monitor.AcceptsCompleted);

				Console.WriteLine("Receives posted: " + _monitor.ReceivesPosted);
				Console.WriteLine("Receives completed: " + _monitor.ReceivesCompleted);

				Console.WriteLine("Sends posted: " + _monitor.SendsPosted);
				Console.WriteLine("Sends completed: " + _monitor.SendsCompleted);

				Console.WriteLine("Clients closed: " + _monitor.ClientsClosed);
			}
		}
	}
}