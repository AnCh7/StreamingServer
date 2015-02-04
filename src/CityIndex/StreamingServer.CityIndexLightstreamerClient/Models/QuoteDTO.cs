#region Usings

using System;

#endregion

namespace StreamingServer.CityIndexLightstreamerClient.Models
{
	/// <summary>
	///     A quote for a specific order request
	/// </summary>
	public class QuoteDTO
	{
		/// <summary>
		///     The unique ID of the Quote
		/// </summary>
		public int QuoteId { get; set; }

		/// <summary>
		///     The ID of the Order that the Quote is related to
		/// </summary>
		public int OrderId { get; set; }

		/// <summary>
		///     The Market the Quote is related to
		/// </summary>
		public int MarketId { get; set; }

		/// <summary>
		///     The Price of the original Order request for a Buy
		/// </summary>
		public decimal BidPrice { get; set; }

		/// <summary>
		///     The amount the bid price will be adjusted to become an order when
		///     the customer is buying (BidPrice + BidAdjust = BuyPrice)
		/// </summary>
		public decimal BidAdjust { get; set; }

		/// <summary>
		///     The Price of the original Order request for a Sell
		/// </summary>
		public decimal OfferPrice { get; set; }

		/// <summary>
		///     The amount the offer price will be adjusted to become an order
		///     when the customer is selling (OfferPrice + OfferAdjust = OfferPrice)
		/// </summary>
		public decimal OfferAdjust { get; set; }

		/// <summary>
		///     The Quantity is the number of units for the trade i.e
		///     CFD Quantity = Number of CFDs to Buy or Sell , FX Quantity = amount in base currency
		/// </summary>
		public decimal Quantity { get; set; }

		/// <summary>
		///     The system internal ID for the ISO Currency.
		///     An API call will be available in the near future to look up the equivalent ISO Code
		/// </summary>
		public int CurrencyId { get; set; }

		/// <summary>
		///     The Status ID of the Quote. An API call will be available in the near future to look up the Status values.
		/// </summary>
		public int StatusId { get; set; }

		/// <summary>
		///     The quote type ID
		/// </summary>
		public int TypeId { get; set; }

		/// <summary>
		///     The timestamp the quote was requested. Always expressed in UTC
		/// </summary>
		public string RequestDateTimeUTC { get; set; }

		/// <summary>
		///     The timestamp the quote was approved. Always expressed in UTC
		/// </summary>
		public string ApprovalDateTimeUTC { get; set; }

		/// <summary>
		///     Amount of time in seconds the quote is valid for
		/// </summary>
		public int BreathTimeSecs { get; set; }

		/// <summary>
		///     Is the quote oversize
		/// </summary>
		public bool IsOversize { get; set; }

		/// <summary>
		///     The reason for generating the quote
		/// </summary>
		public int ReasonId { get; set; }

		/// <summary>
		///     The trading account identifier that generated the quote
		/// </summary>
		public int TradingAccountId { get; set; }

		public override string ToString()
		{
			return String.Join("/",
				QuoteId,
				OrderId,
				MarketId,
				BidPrice,
				BidAdjust,
				OfferPrice,
				OfferAdjust,
				Quantity,
				CurrencyId,
				StatusId,
				TypeId,
				RequestDateTimeUTC,
				ApprovalDateTimeUTC,
				BreathTimeSecs,
				IsOversize,
				ReasonId,
				TradingAccountId);
		}
	}
}