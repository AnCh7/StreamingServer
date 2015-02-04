#region Usings

using System;

using StreamingServer.LightstreamerClient.EventArguments;

#endregion

namespace StreamingServer.LightstreamerClient.Interfaces
{
	public interface IStreamingListener : IDisposable
	{
		string Topic { get; }

		string Adapter { get; }

		void Start(int phase);

		void Stop();
	}

	public interface IStreamingListener<TDto> : IStreamingListener where TDto : class
	{
		event EventHandler<MessageEventArgs<TDto>> MessageReceived;
	}
}
