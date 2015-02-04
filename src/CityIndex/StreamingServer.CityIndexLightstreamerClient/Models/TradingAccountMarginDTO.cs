#region Usings

using System;

#endregion

namespace StreamingServer.CityIndexLightstreamerClient.Models
{
	/// <summary>
	///     This is only applicable to the GTS back-end. The current margin and
	///     other account balance data for a specific client account used in the
	///     [TradingAccountMargin](http://docs.labs.cityindex.com/Content/Streaming Data/TradingAccountMargin.htm) stream.
	/// </summary>
	public class TradingAccountMarginDTO
	{
		/// <summary>
		///     Cash balance expressed in the clients base currency
		/// </summary>
		public decimal Cash { get; set; }

		/// <summary>
		///     The client account's total margin requirement expressed in base currency
		/// </summary>
		public decimal Margin { get; set; }

		/// <summary>
		///     Margin indicator expressed as a percentage
		/// </summary>
		public decimal MarginIndicator { get; set; }

		/// <summary>
		///     Net equity expressed in the clients base currency
		/// </summary>
		public decimal NetEquity { get; set; }

		/// <summary>
		///     Open trade equity (open / unrealised PNL) expressed in the client's base currency
		/// </summary>
		public decimal OpenTradeEquity { get; set; }

		/// <summary>
		///     Tradable funds expressed in the client's base currency
		/// </summary>
		public decimal TradeableFunds { get; set; }

		/// <summary>
		///     N/A
		/// </summary>
		public decimal PendingFunds { get; set; }

		/// <summary>
		///     Trading resource expressed in the client's base currency
		/// </summary>
		public decimal TradingResource { get; set; }

		/// <summary>
		///     Total margin requirement expressed in the client's base currency
		/// </summary>
		public decimal TotalMarginRequirement { get; set; }

		/// <summary>
		///     The identifier for the trading account
		/// </summary>
		public int TradingAccountId { get; set; }

		/// <summary>
		///     The clients base currency ID
		/// </summary>
		public int CurrencyId { get; set; }

		/// <summary>
		///     The clients base currency ISO code
		/// </summary>
		public string CurrencyISO { get; set; }

		public override string ToString()
		{
			return String.Join("/",
							   Cash,
							   Margin,
							   MarginIndicator,
							   NetEquity,
							   OpenTradeEquity,
							   TradeableFunds,
							   PendingFunds,
							   TradingResource,
							   TotalMarginRequirement,
							   TradingAccountId,
							   CurrencyId,
							   CurrencyISO);
		}
	}
}
