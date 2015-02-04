#region Using

using System;

#endregion

namespace StreamingServer.CityIndexLightstreamerClient.Models
{
	/// <summary>
	///     The current margin requirement and open trade equity (OTE) of an order, used in the TradeMargin stream
	/// </summary>
	public class TradeMarginDTO
	{
		/// <summary>
		///     The client account this message relates to
		/// </summary>
		public int ClientAccountId { get; set; }

		/// <summary>
		///     The order direction, 1 == Buy and 0 == Sell
		/// </summary>
		public int DirectionId { get; set; }

		/// <summary>
		///     The margin requirement converted to the correct currency for this order
		/// </summary>
		public decimal MarginRequirementConverted { get; set; }

		/// <summary>
		///     The currency ID of the margin requirement for this order.
		///     See the "Currency ID" section of the CIAPI User Guide for a listing
		///     of the currency IDs
		/// </summary>
		public int MarginRequirementConvertedCurrencyId { get; set; }

		/// <summary>
		///     The currency ISO code of the margin requirement for this order
		/// </summary>
		public string MarginRequirementConvertedCurrencyISOCode { get; set; }

		/// <summary>
		///     The market ID the order is on
		/// </summary>
		public int MarketId { get; set; }

		/// <summary>
		///     The market type ID. 1 = Option Market; 2 = Ordinary Market; 4 = Binary Market
		/// </summary>
		public int MarketTypeId { get; set; }

		/// <summary>
		///     The margin multiplier
		/// </summary>
		public decimal Multiplier { get; set; }

		/// <summary>
		///     The Order ID
		/// </summary>
		public int OrderId { get; set; }

		/// <summary>
		///     The Open Trade Equity converted to the correct currency for this order
		/// </summary>
		public decimal OTEConverted { get; set; }

		/// <summary>
		///     The currency ID of the OTE for this order. See the "Currency ID" section of the CIAPI User Guide for a listing of
		///     the
		///     currency IDs
		/// </summary>
		public int OTEConvertedCurrencyId { get; set; }

		/// <summary>
		///     The currency ISO code of the OTE for this order
		/// </summary>
		public string OTEConvertedCurrencyISOCode { get; set; }

		/// <summary>
		///     The price the calculation was performed at
		/// </summary>
		public decimal PriceCalculatedAt { get; set; }

		/// <summary>
		///     The price the order was taken at
		/// </summary>
		public decimal PriceTakenAt { get; set; }

		/// <summary>
		///     The price the calculation was performed at in ticks
		/// </summary>
		public decimal PriceCalculatedAtInTicks { get; set; }

		/// <summary>
		///     The price the order was taken at in ticks
		/// </summary>
		public decimal PriceTakenAtInTicks { get; set; }

		/// <summary>
		///     The quantity of the order
		/// </summary>
		public decimal Quantity { get; set; }

		public override string ToString()
		{
			return String.Join("/",
				ClientAccountId,
				DirectionId,
				MarginRequirementConverted,
				MarginRequirementConvertedCurrencyId,
				MarginRequirementConvertedCurrencyISOCode,
				MarketId,
				MarketTypeId,
				Multiplier,
				OrderId,
				OTEConverted,
				OTEConvertedCurrencyId,
				OTEConvertedCurrencyISOCode,
				PriceCalculatedAt,
				PriceTakenAt,
				PriceCalculatedAtInTicks,
				PriceTakenAtInTicks,
				Quantity);
		}
	}
}