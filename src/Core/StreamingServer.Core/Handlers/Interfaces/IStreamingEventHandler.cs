#region Usings

using System;
using System.Collections.Concurrent;
using StreamingServer.LightstreamerClient.EventArguments;
using StreamingServer.LightstreamerClient.Interfaces;

#endregion

namespace StreamingServer.Core.Handlers.Interfaces
{
	public interface IStreamingEventHandler : IDisposable
	{
		ConcurrentQueue<string> Queue { get; set; }

		IStreamingListener Listener { get; set; }
	}

	public interface IStreamingEventHandler<T> : IStreamingEventHandler where T : class
	{
		void OnMessageReceived(object sender, MessageEventArgs<T> e);
	}
}