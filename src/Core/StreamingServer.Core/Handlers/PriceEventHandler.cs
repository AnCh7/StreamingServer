#region Usings

using System.Collections.Concurrent;
using StreamingServer.CityIndexLightstreamerClient.Models;
using StreamingServer.Core.Handlers.Interfaces;
using StreamingServer.LightstreamerClient.EventArguments;
using StreamingServer.LightstreamerClient.Interfaces;

#endregion

namespace StreamingServer.Core.Handlers
{
	public class PriceEventHandler : IStreamingEventHandler<PriceDTO>
	{
		public ConcurrentQueue<string> Queue { get; set; }
		public IStreamingListener Listener { get; set; }

		public PriceEventHandler(IStreamingListener<PriceDTO> listener)
		{
			Listener = listener;
			listener.MessageReceived += OnMessageReceived;

			Queue = new ConcurrentQueue<string>();
		}

		public void OnMessageReceived(object sender, MessageEventArgs<PriceDTO> e)
		{
			Queue.Enqueue(e.Data.ToString());
		}

		public void Dispose()
		{
			var l = (IStreamingListener<PriceDTO>)Listener;
			l.MessageReceived -= OnMessageReceived;

			string ignored;
			while (Queue.TryDequeue(out ignored))
			{
			}
		}
	}
}