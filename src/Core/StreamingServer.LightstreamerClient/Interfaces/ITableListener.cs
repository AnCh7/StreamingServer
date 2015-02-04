#region Usings

using System;

using Lightstreamer.DotNet.Client;

using StreamingServer.LightstreamerClient.EventArguments;

#endregion

namespace StreamingServer.LightstreamerClient.Interfaces
{
	public interface ITableListener<TDto> : IHandyTableListener where TDto : class, new()
	{
		event EventHandler<MessageEventArgs<TDto>> MessageReceived;
	}
}
