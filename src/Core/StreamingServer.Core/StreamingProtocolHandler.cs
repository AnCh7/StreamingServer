#region Usings

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StreamingServer.CityIndexLightstreamerClient;
using StreamingServer.Common.Logging;
using StreamingServer.Core.Handlers;
using StreamingServer.Core.Handlers.Interfaces;
using StreamingServer.SocketServer.Interfaces;
using StreamingServer.SocketServer.Models;

#endregion

namespace StreamingServer.Core
{
	public class StreamingProtocolHandler : IProtocolHandler
	{
		private readonly ILogger _logger;
		private readonly IStreamingClient _streamingClient;

		private readonly ConcurrentDictionary<Request, IStreamingEventHandler> _subscriptions;

		public StreamingProtocolHandler(ILogger logger, IStreamingClient streamingClient)
		{
			_logger = logger;
			_streamingClient = streamingClient;

			_subscriptions = new ConcurrentDictionary<Request, IStreamingEventHandler>();
		}

		/// <exception cref="OverflowException">The dictionary already contains the maximum number of elements, <see cref="F:System.Int32.MaxValue" />.</exception>
		/// <exception cref="NotSupportedException">Condition. </exception>
		public void HandleRequest(Request request)
		{
			switch (request.RequestType)
			{
				case "NEWS":
					if (!String.IsNullOrEmpty(request.Parameters))
					{
						var listenerNewsHeadlines = _streamingClient.BuildNewsHeadlinesListener(request.UserName, request.SessionToken, request.Parameters);
						var newsEventHandler = new NewsEventHandler(listenerNewsHeadlines);

						if (!_subscriptions.TryAdd(request, newsEventHandler))
						{
							_logger.Error("Can't subscribe to NEWS datafeed");
						}
					}

					_logger.Error("Can't subscribe to NEWS datafeed. News category wasn't provided");
					break;

				case "PRICES":
					var splitted = request.Parameters.Split(new[] {"."}, StringSplitOptions.RemoveEmptyEntries);
					if (splitted.Any())
					{
						var listenerPrices = _streamingClient.BuildPricesListener(request.UserName, request.SessionToken, splitted);
						var priceEventHandler = new PriceEventHandler(listenerPrices);

						if (!_subscriptions.TryAdd(request, priceEventHandler))
						{
							_logger.Error("Can't subscribe to PRICES datafeed");
						}
					}

					_logger.Error("Can't subscribe to PRICES datafeed. Market ids weren't provided.");
					break;

				case "QUOTES":
					var listenerQuotes = _streamingClient.BuildQuotesListener(request.UserName, request.SessionToken);
					var quoteEventHandler = new QuoteEventHandler(listenerQuotes);

					if (!_subscriptions.TryAdd(request, quoteEventHandler))
					{
						_logger.Error("Can't subscribe to QUOTES datafeed");
					}
					break;

				case "CLIENTACCOUNTMARGIN":
					var listenerClientAccountMargin = _streamingClient.BuildClientAccountMarginListener(request.UserName, request.SessionToken);
					var clientAccountMarginEventHandler = new ClientAccountMarginEventHandler(listenerClientAccountMargin);

					if (!_subscriptions.TryAdd(request, clientAccountMarginEventHandler))
					{
						_logger.Error("Can't subscribe to CLIENTACCOUNTMARGIN datafeed");
					}
					break;

				case "ORDERS":
					var listenerOrders = _streamingClient.BuildOrdersListener(request.UserName, request.SessionToken);
					var orderEventHandler = new OrderEventHandler(listenerOrders);

					if (!_subscriptions.TryAdd(request, orderEventHandler))
					{
						_logger.Error("Can't subscribe to ORDERS datafeed");
					}
					break;

				case "DEFAULTPRICES":
					if (!String.IsNullOrEmpty(request.Parameters))
					{
						int accountOperatorId;
						Int32.TryParse(request.Parameters, out accountOperatorId);

						if (accountOperatorId != 0)
						{
							var listenerDefaultPrices = _streamingClient.BuildDefaultPricesListener(request.UserName, request.SessionToken, accountOperatorId);
							var priceEventHandler = new PriceEventHandler(listenerDefaultPrices);

							if (!_subscriptions.TryAdd(request, priceEventHandler))
							{
								_logger.Error("Can't subscribe to DEFAULTPRICES datafeed");
							}
						}
						_logger.Error("Can't subscribe to DEFAULTPRICES datafeed. Can't parse account operator Id.");
					}
					else
					{
						_logger.Error("Can't subscribe to DEFAULTPRICES datafeed. Account operator Id wasn't provided.");
					}
					break;

				case "TRADEMARGIN":
					var listenerTradeMargin = _streamingClient.BuildTradeMarginListener(request.UserName, request.SessionToken);
					var tradeMarginEventHandler = new TradeMarginEventHandler(listenerTradeMargin);

					if (!_subscriptions.TryAdd(request, tradeMarginEventHandler))
					{
						_logger.Error("Can't subscribe to TRADEMARGIN datafeed");
					}
					break;

				case "TRADINGACCOUNTMARGIN":
					var listenerTradingAccountMargin = _streamingClient.BuildTradingAccountMarginListener(request.UserName, request.SessionToken);
					var tradingAccountMarginEventHandler = new TradingAccountMarginEventHandler(listenerTradingAccountMargin);

					if (!_subscriptions.TryAdd(request, tradingAccountMarginEventHandler))
					{
						_logger.Error("Can't subscribe to TRADINGACCOUNTMARGIN datafeed");
					}
					break;

				default:
					throw new NotSupportedException();
			}
		}

		/// <exception cref="KeyNotFoundException">Condition. </exception>
		public void HandleResponse(Request request, Response response)
		{
			Task.Factory.StartNew(() =>
								  {
									  IStreamingEventHandler listener;
									  if (!_subscriptions.TryGetValue(request, out listener))
									  {
										  throw new KeyNotFoundException();
									  }

									  string result;
									  if (listener.Queue.TryDequeue(out result))
									  {
										  var contentStream = new MemoryStream(Encoding.ASCII.GetBytes(result));
										  response.Content = contentStream;
									  }
								  });
		}

		public void Dispose(Request request)
		{
			Dispose(true, request);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing, Request request)
		{
			if (disposing)
			{
				IStreamingEventHandler listener;
				if (_subscriptions.TryRemove(request, out listener))
				{
					listener.Dispose();
					_streamingClient.TearDownListener(listener.Listener);
				}
			}
		}
	}
}