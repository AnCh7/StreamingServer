#region Usings

using System.Collections.Concurrent;
using StreamingServer.CityIndexLightstreamerClient.Models;
using StreamingServer.Core.Handlers.Interfaces;
using StreamingServer.LightstreamerClient.EventArguments;
using StreamingServer.LightstreamerClient.Interfaces;

#endregion

namespace StreamingServer.Core.Handlers
{
	public class TradingAccountMarginEventHandler : IStreamingEventHandler<TradingAccountMarginDTO>
	{
		public ConcurrentQueue<string> Queue { get; set; }
		public IStreamingListener Listener { get; set; }

		public TradingAccountMarginEventHandler(IStreamingListener<TradingAccountMarginDTO> listener)
		{
			Listener = listener;
			listener.MessageReceived += OnMessageReceived;

			Queue = new ConcurrentQueue<string>();
		}

		public void OnMessageReceived(object sender, MessageEventArgs<TradingAccountMarginDTO> e)
		{
			Queue.Enqueue(e.Data.ToString());
		}

		public void Dispose()
		{
			var l = (IStreamingListener<TradingAccountMarginDTO>)Listener;
			l.MessageReceived -= OnMessageReceived;

			string ignored;
			while (Queue.TryDequeue(out ignored))
			{
			}
		}
	}
}