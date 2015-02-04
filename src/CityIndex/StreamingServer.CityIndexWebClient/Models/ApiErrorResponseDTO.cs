namespace StreamingServer.CityIndexWebClient.Models
{
	/// <summary>
	///     The response to an error condition
	/// </summary>
	public class ApiErrorResponseDTO
	{
		/// <summary>
		///     The intended HTTP status code. This will be the same value as the actual
		///     HTTP status code unless the QueryString contains only200=true.
		///     This is useful for JavaScript clients who can only read responses with status code 200
		/// </summary>
		public int HttpStatus { get; set; }

		/// <summary>
		///     This is a description of the ErrorMessage property
		/// </summary>
		public string ErrorMessage { get; set; }

		/// <summary>
		///     The error code
		/// </summary>
		public int ErrorCode { get; set; }
	}
}