#region Usings

using System;

using Lightstreamer.DotNet.Client;

using StreamingServer.Common.JsonSerializer;
using StreamingServer.Common.Logging;
using StreamingServer.LightstreamerClient.EventArguments;
using StreamingServer.LightstreamerClient.Interfaces;

#endregion

namespace StreamingServer.LightstreamerClient
{
	internal class TableListener<TDto> : ITableListener<TDto> where TDto : class, new()
	{
		private readonly ILogger _logger;
		private readonly IDtoConverter<TDto> _messageConverter;
		private readonly IJsonSerializer _serializer;

		private readonly string _topic;
		private readonly int _phase;
		private readonly string _channel;
		private readonly string _adapter;

		public event EventHandler<MessageEventArgs<TDto>> MessageReceived;

		public TableListener(ILogger logger, string adapter, string channel, string topic, int phase, IJsonSerializer serializer)
		{
			_logger = logger;

			_serializer = serializer;
			_messageConverter = new DtoConverter<TDto>(_serializer, _logger);

			_adapter = adapter;
			_channel = channel;
			_phase = phase;
			_topic = topic;
		}

		private static bool IsUpdateNull(IUpdateInfo update)
		{
			for (var i = 1; i < update.NumFields + 1; i++)
			{
				object obj = update.IsValueChanged(i) ? update.GetNewValue(i) : update.GetOldValue(i);
				if (obj != null)
				{
					return false;
				}
			}

			return true;
		}

		void IHandyTableListener.OnUpdate(int itemPos, string itemName, IUpdateInfo update)
		{
			if (IsUpdateNull(update))
			{
				return;
			}
			try
			{
				if (MessageReceived != null)
				{
					var messageData = _messageConverter.Convert(update);
					var e = new MessageEventArgs<TDto>(_adapter, _channel + "." + _topic, messageData, _phase);
					MessageReceived(this, e);
				}
			}
			catch (Exception ex)
			{
				_logger.Error(ex);
				throw;
			}
		}

		void IHandyTableListener.OnRawUpdatesLost(int itemPos, string itemName, int lostUpdates)
		{
			_logger.Debug(string.Format("OnRawUpdatesLost fired -> itemPos: {0} ietmName: {1} lostUpdates:{2}", itemPos, itemName, lostUpdates));
		}

		void IHandyTableListener.OnSnapshotEnd(int itemPos, string itemName)
		{
			_logger.Debug(string.Format("OnSnapshotEnd fired -> itemPos: {0} ietmName: {1}", itemPos, itemName));
		}

		void IHandyTableListener.OnUnsubscr(int itemPos, string itemName)
		{
			_logger.Debug(string.Format("OnUnsubscr fired -> itemPos: {0} ietmName: {1}", itemPos, itemName));
		}

		void IHandyTableListener.OnUnsubscrAll()
		{
			_logger.Debug("OnUnsubscrAll fired");
		}
	}
}
