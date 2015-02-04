namespace StreamingServer.CityIndexWebClient.Models
{
	/// <summary>
	///     Request to create a session (log on).
	/// </summary>
	public class ApiLogOnRequestDTO
	{
		public ApiLogOnRequestDTO(string userName, string password, string appKey, string appVersion)
		{
			UserName = userName;
			Password = password;
			AppKey = appKey;
			AppVersion = appVersion;
		}

		/// <summary>
		///     Username is case sensitive.
		/// </summary>
		public string UserName { get; set; }

		/// <summary>
		///     Password is case sensitive.
		/// </summary>
		public string Password { get; set; }

		/// <summary>
		///     A unique key to identify the client application.
		/// </summary>
		public string AppKey { get; set; }

		/// <summary>
		///     The version of the client application.
		/// </summary>
		public string AppVersion { get; set; }

		/// <summary>
		///     Any client application comments on what to associate with this session. (Optional).
		/// </summary>
		public string AppComments { get; set; }
	}
}
