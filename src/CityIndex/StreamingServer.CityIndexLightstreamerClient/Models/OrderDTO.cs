#region Usings

using System;

#endregion

namespace StreamingServer.CityIndexLightstreamerClient.Models
{
	/// <summary>
	///     An order for a specific Trading Account.
	/// </summary>
	public class OrderDTO
	{
		/// <summary>
		///     The Order identifier.
		/// </summary>
		public int OrderId { get; set; }

		/// <summary>
		///     The Market identifier.
		/// </summary>
		public int MarketId { get; set; }

		/// <summary>
		///     Client account ID.
		/// </summary>
		public int ClientAccountId { get; set; }

		/// <summary>
		///     Trading account ID.
		/// </summary>
		public int TradingAccountId { get; set; }

		/// <summary>
		///     Trade currency ID.
		/// </summary>
		public int CurrencyId { get; set; }

		/// <summary>
		///     Trade currency ISO code.
		/// </summary>
		public string CurrencyISO { get; set; }

		/// <summary>
		///     Direction of the order (1 == buy, 0 == sell).
		/// </summary>
		public int Direction { get; set; }

		/// <summary>
		///     Flag indicating whether the order automatically rolls over.
		///     Only applies to markets where the underlying is a futures contract.
		/// </summary>
		public bool AutoRollover { get; set; }

		/// <summary>
		///     The price at which the order was executed.
		/// </summary>
		public decimal ExecutionPrice { get; set; }

		/// <summary>
		///     The date and time that the order was last changed. Always expressed in UTC.
		/// </summary>
		public string LastChangedTime { get; set; }

		/// <summary>
		///     The open price of the order.
		/// </summary>
		public decimal OpenPrice { get; set; }

		/// <summary>
		///     The date of the order. Always expressed in UTC.
		/// </summary>
		public string OriginalLastChangedDateTime { get; set; }

		/// <summary>
		///     The orders original quantity, before any part / full closures.
		/// </summary>
		public decimal OriginalQuantity { get; set; }

		/// <summary>
		///     Indicates the position of the trade. 1 == LongOrShortOnly, 2 == LongAndShort.
		/// </summary>
		public int? PositionMethodId { get; set; }

		/// <summary>
		///     The current quantity of the order.
		/// </summary>
		public decimal Quantity { get; set; }

		/// <summary>
		///     The type of the order (1 = Trade / 2 = Stop / 3 = Limit).
		///     The table of lookup codes can be found at Lookup Values.
		/// </summary>
		public string Type { get; set; }

		/// <summary>
		///     The order status ID. The table of lookup codes can be found at Lookup Values.
		/// </summary>
		public string Status { get; set; }

		/// <summary>
		///     The order status reason identifier. The table of lookup codes can be found at Lookup Values.
		/// </summary>
		public int ReasonId { get; set; }

		public override string ToString()
		{
			return String.Join("/",
							   OrderId,
							   MarketId,
							   ClientAccountId,
							   TradingAccountId,
							   CurrencyId,
							   CurrencyISO,
							   Direction,
							   AutoRollover,
							   ExecutionPrice,
							   LastChangedTime,
							   OpenPrice,
							   OriginalLastChangedDateTime,
							   OriginalQuantity,
							   PositionMethodId,
							   Quantity,
							   Type,
							   Status,
							   ReasonId);
		}
	}
}
