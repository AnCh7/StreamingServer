#region Usings

using System;
using System.Linq;
using System.Threading;

using Lightstreamer.DotNet.Client;

using StreamingServer.Common.JsonSerializer;
using StreamingServer.Common.Logging;
using StreamingServer.LightstreamerClient.Common;
using StreamingServer.LightstreamerClient.EventArguments;
using StreamingServer.LightstreamerClient.Interfaces;

#endregion

namespace StreamingServer.LightstreamerClient
{
	public sealed class ListenerAdapter<TDto> : IStreamingListener<TDto> where TDto : class, new()
	{
		private readonly ILogger _logger;
		private readonly IJsonSerializer _serializer;
		private readonly ILightstreamerAdapter _lightstreamer;

		private readonly DtoConverter<TDto> _messageConverter;
		private TableListener<TDto> _listener;
		private SubscribedTableKey _subscribedTableKey;

		private readonly string _mode;
		private readonly bool _snapshot;
		private readonly string _adapterSet;
		private readonly string _channel;
		private readonly string _groupOrItemName;

		public event EventHandler<MessageEventArgs<TDto>> MessageReceived;

		public string AdapterSet
		{
			get { return _adapterSet; }
		}

		public string Adapter
		{
			get { return _channel; }
		}

		public string Topic { get; private set; }

		public ListenerAdapter(ILogger logger,
							   string topic,
							   string mode,
							   bool snapshot,
							   ILightstreamerAdapter lightstreamer,
							   IJsonSerializer serializer)
		{
			_logger = logger;
			_serializer = serializer;
			_lightstreamer = lightstreamer;

			_messageConverter = new DtoConverter<TDto>(_serializer, _logger);

			Topic = topic;
			_adapterSet = lightstreamer.AdapterSet;
			_channel = topic.Split('.').First();
			_groupOrItemName = topic.Replace(_channel + ".", "");
			_mode = mode;
			_snapshot = snapshot;
		}

		void IStreamingListener.Start(int phase)
		{
			var text = _groupOrItemName.ToUpper();
			if (_listener != null)
			{
				_listener.MessageReceived -= ListenerMessageReceived;
				((IStreamingListener)this).Stop();
			}

			var fieldList = _messageConverter.GetFieldList();
			var text2 = _channel.ToUpper();

			_listener = new TableListener<TDto>(_logger, _adapterSet.ToUpper(), text2, text, phase, _serializer);
			_listener.MessageReceived += ListenerMessageReceived;

			_logger.Debug(string.Format("Subscribing to group:{0}, schema {1}, dataAdapter {2}, mode {3}, snapshot {4}",
										text,
										fieldList,
										text2,
										_mode.ToUpper(),
										_snapshot));

			var schema = fieldList;
			var mode = _mode.ToUpper();
			var snapshot = _snapshot;
			var simpleTableInfo = new SimpleTableInfo(text, mode, schema, snapshot)
			{
				DataAdapter = text2
			};

			var gate = new ManualResetEvent(false);
			Exception ex = null;

			new Thread(delegate()
			{
				try
				{
					_subscribedTableKey = _lightstreamer.SubscribeTable(simpleTableInfo, _listener, false);
					_logger.Debug(string.Format("Subscribed to table with key: {0}", _subscribedTableKey));
				}
				catch (Exception e)
				{
					ex = e;
				}
				gate.Set();
			}).Start();

			if (ex != null)
			{
				_logger.Error(ex);
				throw ex;
			}
			if (!gate.WaitOne(Settings.DefaultTimeoutMs + 1000))
			{
				_logger.Error(string.Format("Listener taking longer than {0}ms to start: {1}.", Settings.DefaultTimeoutMs, base.GetType().Name));
			}
		}

		void IStreamingListener.Stop()
		{
			if (_subscribedTableKey == null)
			{
				return;
			}

			var text = string.Format("Unsubscribing from table with key: {0}", _subscribedTableKey);
			_logger.Debug(text);

			new Thread(delegate()
			{
				try
				{
					this._lightstreamer.UnsubscribeTable(this._subscribedTableKey);
				}
				catch (Exception ex)
				{
					_logger.Warn(ex);
				}
			})
			{
				Name = "Thread for " + text
			}.Start();
		}

		private void ListenerMessageReceived(object sender, MessageEventArgs<TDto> e)
		{
			if (!_lightstreamer.CheckPhase(e.Phase))
			{
				return;
			}

			var messageReceived = MessageReceived;
			if (messageReceived != null)
			{
				messageReceived(this, e);
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (disposing)
			{}
		}
	}
}
