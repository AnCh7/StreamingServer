#region Usings

using System.Collections.Concurrent;
using StreamingServer.CityIndexLightstreamerClient.Models;
using StreamingServer.Core.Handlers.Interfaces;
using StreamingServer.LightstreamerClient.EventArguments;
using StreamingServer.LightstreamerClient.Interfaces;

#endregion

namespace StreamingServer.Core.Handlers
{
	public class ClientAccountMarginEventHandler : IStreamingEventHandler<ClientAccountMarginDTO>
	{
		public ConcurrentQueue<string> Queue { get; set; }
		public IStreamingListener Listener { get; set; }

		public ClientAccountMarginEventHandler(IStreamingListener<ClientAccountMarginDTO> listener)
		{
			Listener = listener;
			listener.MessageReceived += OnMessageReceived; 
			
			Queue = new ConcurrentQueue<string>();
		}

		public void OnMessageReceived(object sender, MessageEventArgs<ClientAccountMarginDTO> e)
		{
			Queue.Enqueue(e.Data.ToString());
		}

		public void Dispose()
		{
			var l = (IStreamingListener<ClientAccountMarginDTO>)Listener;
			l.MessageReceived -= OnMessageReceived;

			string ignored;
			while (Queue.TryDequeue(out ignored))
			{
			}
		}
	}
}
