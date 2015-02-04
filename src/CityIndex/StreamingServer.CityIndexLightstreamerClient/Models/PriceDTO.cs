#region Usings

using System;

#endregion

namespace StreamingServer.CityIndexLightstreamerClient.Models
{
	/// <summary>
	///     A Price for a specific Market.
	/// </summary>
	public class PriceDTO
	{
		/// <summary>
		///     The Market that the Price is related to.
		/// </summary>
		public int MarketId { get; set; }

		/// <summary>
		///     The date of the Price. Always expressed in UTC.
		/// </summary>
		public string TickDate { get; set; }

		/// <summary>
		///     The current Bid price (price at which the customer can sell).
		/// </summary>
		public decimal Bid { get; set; }

		/// <summary>
		///     The current Offer price (price at which the customer can buy, sometimes referred to as Ask price).
		/// </summary>
		public decimal Offer { get; set; }

		/// <summary>
		///     The current mid price.
		/// </summary>
		public decimal Price { get; set; }

		/// <summary>
		///     The highest price reached for the day.
		/// </summary>
		public decimal High { get; set; }

		/// <summary>
		///     The lowest price reached for the day.
		/// </summary>
		public decimal Low { get; set; }

		/// <summary>
		///     The change since the last price (always positive). See Direction for direction of the change.
		/// </summary>
		public decimal Change { get; set; }

		/// <summary>
		///     The direction of movement since the last price. 1 == up, 0 == down.
		/// </summary>
		public int Direction { get; set; }

		/// <summary>
		///     The Delta of an option. Delta measures the rate of change of option value with respect to changes
		///     in the underlying asset's price. This is null for non-option markets.
		/// </summary>
		public decimal? Delta { get; set; }

		/// <summary>
		///     A measure of an options's price variance over time.
		///     Note: this volatility is a calculated value from a proprietary model.
		///     For non-option markets this is null.
		/// </summary>
		public decimal? ImpliedVolatility { get; set; }

		/// <summary>
		///     A unique ID for this price. Treat as a unique, but random string.
		/// </summary>
		public string AuditId { get; set; }

		/// <summary>
		///     The current status summary for this price.
		///     Values are: 0 = Normal 1 = Indicative 2 = PhoneOnly 3 = Suspended 4 = Closed
		/// </summary>
		public int StatusSummary { get; set; }

		public override string ToString()
		{
			return String.Join("/",
							   MarketId,
							   TickDate,
							   Bid,
							   Offer,
							   Price,
							   High,
							   Low,
							   Change,
							   Direction,
							   Delta,
							   ImpliedVolatility,
							   AuditId,
							   StatusSummary);
		}
	}
}
