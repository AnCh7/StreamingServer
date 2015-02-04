namespace StreamingServer.CityIndexWebClient.Models
{
	/// <summary>
	///     Response to a [LogOn](http://docs.labs.cityindex.com/Content/HTTP Services/LogOn.htm) call.
	/// </summary>
	public class ApiLogOnResponseDTO
	{
		/// <summary>
		///     Your session token (treat as a random string). Session tokens are valid for a set period from the time of their creation.
		///     The period is subject to change, and may vary depending on who you logon as.
		/// </summary>
		public string Session { get; set; }

		/// <summary>
		///     Flag used to indicate whether a password change is needed.
		/// </summary>
		public bool PasswordChangeRequired { get; set; }

		/// <summary>
		///     Flag used to indicate whether the account operator associated with this user is allowed
		///     to access the application.
		/// </summary>
		public bool AllowedAccountOperator { get; set; }
	}
}
