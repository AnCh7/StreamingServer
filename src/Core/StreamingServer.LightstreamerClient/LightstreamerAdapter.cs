#region Usings

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;

using Lightstreamer.DotNet.Client;
using Lightstreamer.DotNet.Client.Log;
using StreamingServer.Common.JsonSerializer;
using StreamingServer.Common.Logging;
using StreamingServer.LightstreamerClient.Common;
using StreamingServer.LightstreamerClient.Enums;
using StreamingServer.LightstreamerClient.EventArguments;
using StreamingServer.LightstreamerClient.Interfaces;
using ILogger = StreamingServer.Common.Logging.ILogger;

#endregion

namespace StreamingServer.LightstreamerClient
{
	public sealed class LightstreamerAdapter : IConnectionListener, ILightstreamerAdapter
	{
		private class ClientStartStop
		{
			private readonly ILightstreamerAdapter _adapter;
			private readonly int _phase;

			public ClientStartStop(int ph, ILightstreamerAdapter adapter)
			{
				_adapter = adapter;
				_phase = ph;
			}

			public void DoStart()
			{
				_adapter.OpenConnection(_phase);
			}

			public void DoStop()
			{
				_adapter.CloseConnection(_phase);
			}
		}

		private readonly IJsonSerializer _serializer;
		private readonly ILogger _logger;
		private readonly LSClient _client;
		private readonly Dictionary<string, IStreamingListener> _currentListeners = new Dictionary<string, IStreamingListener>();

		private static readonly object ConnLock = new object();

		private readonly string _adapterSet;
		private readonly string _password;
		private readonly string _userName;
		private readonly bool _usePolling;
		private readonly string _streamingUri;

		private bool _isPolling;
		private bool _reconnect;
		private int _phase;
		private int _lastDelay = 1;
		private bool _disposed;

		public event EventHandler<ConnectionStatusEventArgs> StatusUpdate;

		public string AdapterSet
		{
			get { return _adapterSet; }
		}

		public bool Connected { get; private set; }

		public int ListenerCount
		{
			get { return _currentListeners.Count; }
		}

		public LightstreamerAdapter(ILogger logger,
							 IJsonSerializer serializer,
							 string streamingUri,
							 string userName,
							 string password,
							 string adapterSet,
							 bool usePolling)

		{
			ServicePointManager.DefaultConnectionLimit += 2;
			_client = new LSClient();

			_logger = logger;
			_serializer = serializer;

			_streamingUri = streamingUri;
			_userName = userName;
			_password = password;
			_adapterSet = adapterSet;
			_usePolling = usePolling;
		}

		public SubscribedTableKey SubscribeTable<TDto>(SimpleTableInfo simpleTableInfo, ITableListener<TDto> listener, bool b) where TDto : class, new()
		{
			return _client.SubscribeTable(simpleTableInfo, listener, b);
		}

		public void UnsubscribeTable(SubscribedTableKey subscribedTableKey)
		{
			_client.UnsubscribeTable(subscribedTableKey);
		}

		/// <exception cref="Exception">Timeout starting lightstreamer thread</exception>
		public void Start()
		{
			var connLock = ConnLock;
			ClientStartStop execute;
			lock (connLock)
			{
				_phase++;
				execute = new ClientStartStop(_phase, this);
			}

			var gate = new ManualResetEvent(false);
			Exception ex = null;
			new Thread(delegate()
			{
				try
				{
					execute.DoStart();
				}
				catch (Exception e)
				{
					ex = e;
				}

				gate.Set();
			})
			{
				Name = "LightStreamerStartThread"
			}.Start();

			if (!gate.WaitOne(Settings.DefaultTimeoutMs + 1000))
			{
				throw new Exception("Timeout starting lightstreamer thread");
			}

			if (ex != null)
			{
				throw ex;
			}
		}

		public void Stop()
		{
			var connLock = ConnLock;
			ClientStartStop execute;
			lock (connLock)
			{
				_phase++;
				execute = new ClientStartStop(_phase, this);
			}

			var gate = new ManualResetEvent(false);
			new Thread(delegate()
			{
				execute.DoStop();
				gate.Set();
			})
			{
				Name = "LightStreamerStopThread"
			}.Start();

			gate.WaitOne();
		}

		void IConnectionListener.OnConnectionEstablished()
		{
			const string message = "Connected to Lightstreamer Server...";

			_logger.Info(message);
			OnStatusUpdate(_phase, ConnectionStatus.Connected, message);
		}

		void IConnectionListener.OnSessionStarted(bool isPolling)
		{
			_isPolling = isPolling;
			const string message = "Lightstreamer is pushing...";

			ConnectionStatus status;
			if (isPolling)
			{
				status = ConnectionStatus.Polling;
			}
			else
			{
				status = ConnectionStatus.Streaming;
			}

			_logger.Info(message);
			OnStatusUpdate(_phase, status, message);
		}

		void IConnectionListener.OnNewBytes(long bytes)
		{
			// ignored
		}

		void IConnectionListener.OnDataError(PushServerException e)
		{
			_logger.Info("Data error");
			OnStatusUpdate(_phase, ConnectionStatus.Error, "Data error");
		}

		void IConnectionListener.OnActivityWarning(bool warningOn)
		{
			if (warningOn)
			{
				_logger.Info("Connection stalled");
				OnStatusUpdate(_phase, ConnectionStatus.Stalled, "Connection stalled");
				return;
			}

			((IConnectionListener)this).OnSessionStarted(_isPolling);
		}

		void IConnectionListener.OnClose()
		{
			_logger.Info("Connection closed");
			OnStatusUpdate(_phase, ConnectionStatus.Disconnected, "Connection closed");
			if (_reconnect)
			{
				if (!CheckPhase(_phase))
				{
					return;
				}

				Start();
				_reconnect = false;
			}
		}

		void IConnectionListener.OnEnd(int cause)
		{
			_logger.Info("Connection forcibly closed");

			OnStatusUpdate(_phase, ConnectionStatus.Disconnected, "Connection forcibly closed");
			_reconnect = true;
		}

		void IConnectionListener.OnFailure(PushServerException e)
		{
			_logger.Info("Server failure" + e);
			OnStatusUpdate(_phase, ConnectionStatus.Disconnected, "Server failure" + e);
			_reconnect = true;
		}

		void IConnectionListener.OnFailure(PushConnException e)
		{
			_logger.Info("Connection failure " + e);
			OnStatusUpdate(_phase, ConnectionStatus.Disconnected, "Connection failure " + e);
			_reconnect = true;
		}

		/// <exception cref="ObjectDisposedException">Condition. </exception>
		public void TearDownListener(IStreamingListener listener)
		{
			if (_disposed)
			{
				throw new ObjectDisposedException(base.GetType().FullName);
			}

			var currentListeners = _currentListeners;
			lock (currentListeners)
			{
				if (_currentListeners.ContainsValue(listener))
				{
					_currentListeners.Remove(listener.Topic);
				}

				new Thread(listener.Stop).Start();
			}
		}

		/// <exception cref="ObjectDisposedException">Condition. </exception>
		/// <exception cref="KeyNotFoundException">The property is retrieved and <paramref name="key" /> does not exist in the collection.</exception>
		public IStreamingListener<TDto> BuildListener<TDto>(string topic, string mode, bool snapshot) where TDto : class, new()
		{
			if (_disposed)
			{
				throw new ObjectDisposedException(base.GetType().FullName);
			}

			if (_currentListeners.ContainsKey(topic))
			{
				return (IStreamingListener<TDto>)_currentListeners[topic];
			}

			IStreamingListener listener = new ListenerAdapter<TDto>(_logger, topic, mode, snapshot, this, _serializer);
			_currentListeners.Add(topic, listener);

			new Thread(delegate()
			{
				listener.Start(this._phase);
			}).Start();

			return (IStreamingListener<TDto>)_currentListeners[topic];
		}

		private void OnStatusUpdate(int ph, ConnectionStatus status, string message)
		{
			if (!CheckPhase(ph))
			{
				return;
			}

			var statusUpdate = StatusUpdate;
			if (statusUpdate == null)
			{
				return;
			}

			var e = new ConnectionStatusEventArgs(message, status);
			statusUpdate(this, e);
		}

		private void PauseAndRetryStartClient(int ph)
		{
			_lastDelay *= 2;
			for (var i = _lastDelay; i > 0; i--)
			{
				if (!CheckPhase(ph))
				{
					return;
				}
				if (!NetworkInterface.GetIsNetworkAvailable())
				{
					_logger.Info("Network unavailble, next check in " + i + " seconds");
					OnStatusUpdate(ph, ConnectionStatus.Connecting, "Network unavailble, next check in " + i + " seconds");
				}
				else
				{
					_logger.Info("Connection failed, retrying in " + i + " seconds");
					OnStatusUpdate(ph, ConnectionStatus.Connecting, "Connection failed, retrying in " + i + " seconds");
				}

				Thread.Sleep(1000);
			}
			if (!CheckPhase(_phase))
			{
				return;
			}

			Start();
		}

		public bool CheckPhase(int ph)
		{
			var connLock = ConnLock;

			bool result;
			lock (connLock)
			{
				result = (ph == _phase);
			}

			return result;
		}

		void ILightstreamerAdapter.OpenConnection(int ph)
		{
			if (!NetworkInterface.GetIsNetworkAvailable())
			{
				PauseAndRetryStartClient(ph);
				return;
			}
			try
			{
				if (!CheckPhase(ph))
				{
					return;
				}

				_logger.Info("Connecting to " + _streamingUri);
				OnStatusUpdate(ph, ConnectionStatus.Connecting, "Connecting to " + _streamingUri);
				var info = new ConnectionInfo
				{
					PushServerUrl = _streamingUri.TrimEnd('/'),
					Adapter = _adapterSet,
					User = _userName,
					Password = _password,
					Constraints =
					{
						MaxBandwidth = 999999.0
					},
					Polling = _usePolling
				};
				try
				{
					_client.OpenConnection(info, this);
					Connected = true;
				}
				catch (PushUserException ex)
				{
					if (ex.Message == "Requested Adapter Set not available")
					{
						throw new Exception(string.Format("Adapter set {0} is not available", _adapterSet), ex);
					}

					throw;
				}

				foreach (var current in _currentListeners)
				{
					current.Value.Start(_phase);
				}

				Connected = true;
				_lastDelay = 1;
			}
			catch (PushConnException e)
			{
				PauseAndRetryStartClient(ph);
			}
			catch (SubscrException e)
			{
				PauseAndRetryStartClient(ph);
			}
		}

		void ILightstreamerAdapter.CloseConnection(int ph)
		{
			if (!CheckPhase(ph))
			{
				return;
			}

			_client.CloseConnection();
			Connected = false;
			_logger.Info("Disconnected");
			OnStatusUpdate(ph, ConnectionStatus.Disconnected, "Disconnected");
		}

		public void Dispose()
		{
			_disposed = true;
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (disposing)
			{
				Stop();
				ServicePointManager.DefaultConnectionLimit -= 2;
			}
		}
	}
}
