#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using StreamingServer.CityIndexLightstreamerClient.Models;
using StreamingServer.Common.JsonSerializer;
using StreamingServer.Common.Logging;
using StreamingServer.LightstreamerClient;
using StreamingServer.LightstreamerClient.EventArguments;
using StreamingServer.LightstreamerClient.Interfaces;

#endregion

namespace StreamingServer.CityIndexLightstreamerClient
{
	public interface IStreamingClient : IDisposable
	{
		void Init(string streamingUri, bool usePolling);

		event EventHandler<ConnectionStatusEventArgs> StatusChanged;

		IStreamingListener<PriceDTO> BuildPricesListener(string userName, string password, string[] marketIds);

		IStreamingListener<NewsDTO> BuildNewsHeadlinesListener(string userName, string password, string category);

		IStreamingListener<QuoteDTO> BuildQuotesListener(string userName, string password);

		IStreamingListener<ClientAccountMarginDTO> BuildClientAccountMarginListener(string userName, string password);

		IStreamingListener<OrderDTO> BuildOrdersListener(string userName, string password);

		IStreamingListener<PriceDTO> BuildDefaultPricesListener(string userName, string password, int accountOperatorId);

		IStreamingListener<TradeMarginDTO> BuildTradeMarginListener(string userName, string password);

		IStreamingListener<TradingAccountMarginDTO> BuildTradingAccountMarginListener(string userName, string password);

		void TearDownListener(IStreamingListener listener);
	}

	public class StreamingClient : IStreamingClient
	{
		private const string DataAdapter = "STREAMINGALL";

		private readonly IJsonSerializer _serializer;
		private readonly ILogger _logger;
		private readonly Dictionary<string, ILightstreamerAdapter> _adapters;

		private string _streamingUri;
		private bool _usePolling;
		private bool _disposed;

		public event EventHandler<ConnectionStatusEventArgs> StatusChanged;

		public StreamingClient(ILogger logger, IJsonSerializer serializer)
		{
			_adapters = new Dictionary<string, ILightstreamerAdapter>();

			_logger = logger;
			_serializer = serializer;
		}

		public void Init(string streamingUri, bool usePolling)
		{
			_usePolling = usePolling;
			_streamingUri = streamingUri;
		}

		/// <exception cref="RegexMatchTimeoutException">A time-out occurred. For more information about time-outs, see the Remarks section.</exception>
		public IStreamingListener<NewsDTO> BuildNewsHeadlinesListener(string userName, string password, string category)
		{
			var topic = Regex.Replace("NEWS.HEADLINES.{category}", "{category}", category);
			return BuildListener<NewsDTO>(userName, password, DataAdapter, "MERGE", true, topic);
		}

		/// <exception cref="RegexMatchTimeoutException">A time-out occurred. For more information about time-outs, see the Remarks section.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="source" /> is null.</exception>
		/// <exception cref="ArgumentException">A regular expression parsing error occurred.</exception>
		public IStreamingListener<PriceDTO> BuildPricesListener(string userName, string password, string[] marketIds)
		{
			var topic = string.Join(" ",
									(from t in marketIds
									 select Regex.Replace("PRICES.PRICE.{marketIds}", "{marketIds}", t)).ToArray<string>());
			return BuildListener<PriceDTO>(userName, password, DataAdapter, "MERGE", true, topic);
		}

		public IStreamingListener<QuoteDTO> BuildQuotesListener(string userName, string password)
		{
			const string topic = "QUOTES.QUOTES";
			return BuildListener<QuoteDTO>(userName, password, DataAdapter, "MERGE", true, topic);
		}

		public IStreamingListener<ClientAccountMarginDTO> BuildClientAccountMarginListener(string userName, string password)
		{
			const string topic = "CLIENTACCOUNTMARGIN.CLIENTACCOUNTMARGIN";
			return BuildListener<ClientAccountMarginDTO>(userName, password, DataAdapter, "MERGE", true, topic);
		}

		public IStreamingListener<OrderDTO> BuildOrdersListener(string userName, string password)
		{
			const string topic = "ORDERS.ORDERS";
			return BuildListener<OrderDTO>(userName, password, DataAdapter, "MERGE", true, topic);
		}

		public IStreamingListener<PriceDTO> BuildDefaultPricesListener(string userName, string password, int accountOperatorId)
		{
			return BuildListener<PriceDTO>(userName, password, "CITYINDEXSTREAMINGDEFAULTPRICES", "MERGE", true,
										   "PRICES.AC" + accountOperatorId);
		}

		public IStreamingListener<TradeMarginDTO> BuildTradeMarginListener(string userName, string password)
		{
			const string topic = "TRADEMARGIN.TRADEMARGIN";
			return BuildListener<TradeMarginDTO>(userName, password, DataAdapter, "RAW", false, topic);
		}

		public IStreamingListener<TradingAccountMarginDTO> BuildTradingAccountMarginListener(string userName, string password)
		{
			const string topic = "TRADINGACCOUNTMARGIN.TRADINGACCOUNTMARGIN";
			return BuildListener<TradingAccountMarginDTO>(userName,
														  password,
														  DataAdapter,
														  "RAW",
														  false,
														  topic);
		}

		/// <exception cref="ObjectDisposedException">Condition. </exception>
		/// <exception cref="KeyNotFoundException">
		///     The property is retrieved and <paramref name="key" /> does not exist in the collection.
		/// </exception>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public void TearDownListener(IStreamingListener listener)
		{
			if (_disposed)
			{
				throw new ObjectDisposedException(GetType().FullName);
			}
			if (!_adapters.ContainsKey(listener.Adapter))
			{
				return;
			}

			var clientAdapter = _adapters[listener.Adapter];
			clientAdapter.TearDownListener(listener);

			if (clientAdapter.ListenerCount == 0)
			{
				_adapters.Remove(listener.Adapter);
				clientAdapter.Dispose();
			}
		}

		public void Dispose()
		{
			_disposed = true;
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				foreach (var current in _adapters)
				{
					current.Value.Dispose();
				}
			}
		}

		/// <exception cref="Exception">A delegate callback throws an exception. </exception>
		protected virtual void OnStatusChanged(object sender, ConnectionStatusEventArgs e)
		{
			var statusChanged = StatusChanged;

			if (statusChanged != null)
			{
				statusChanged(sender, e);
			}
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		private IStreamingListener<TDto> BuildListener<TDto>(string userName,
															 string password,
															 string dataAdapter,
															 string mode,
															 bool snapshot,
															 string topic) where TDto : class, new()
		{
			if (_disposed)
			{
				throw new ObjectDisposedException(GetType().FullName);
			}

			if (!_adapters.ContainsKey(dataAdapter))
			{
				LightstreamerAdapter lightstreamerAdapter = null;

				try
				{
					lightstreamerAdapter = new LightstreamerAdapter(_logger, _serializer, _streamingUri, userName, password,
																	dataAdapter, _usePolling);
					lightstreamerAdapter.StatusUpdate += OnStatusChanged;

					_adapters.Add(dataAdapter, lightstreamerAdapter);

					lightstreamerAdapter.Start();
				}
				catch
				{
					if (lightstreamerAdapter != null)
					{
						lightstreamerAdapter.Dispose();
					}

					throw;
				}
			}

			_logger.Debug("StreamingClient created for " + string.Format("{0} {1}", userName, topic));

			return _adapters[dataAdapter].BuildListener<TDto>(topic, mode, snapshot);
		}
	}
}