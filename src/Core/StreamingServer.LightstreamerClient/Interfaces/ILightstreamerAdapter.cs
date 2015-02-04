#region Usings

using System;

using Lightstreamer.DotNet.Client;

using StreamingServer.LightstreamerClient.EventArguments;

#endregion

namespace StreamingServer.LightstreamerClient.Interfaces
{
	public interface ILightstreamerAdapter : IDisposable
	{
		event EventHandler<ConnectionStatusEventArgs> StatusUpdate;

		string AdapterSet { get; }

		bool Connected { get; }

		int ListenerCount { get; }

		bool CheckPhase(int ph);

		void TearDownListener(IStreamingListener listener);

		IStreamingListener<TDto> BuildListener<TDto>(string topic, string mode, bool snapshot) where TDto : class, new();

		SubscribedTableKey SubscribeTable<TDto>(SimpleTableInfo simpleTableInfo, ITableListener<TDto> listener, bool b) where TDto : class, new();

		void UnsubscribeTable(SubscribedTableKey subscribedTableKey);

		void OpenConnection(int phase);

		void CloseConnection(int phase);
	}
}
