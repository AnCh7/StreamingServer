#region Usings

using System;

#endregion

namespace StreamingServer.CityIndexLightstreamerClient.Models
{
	/// <summary>
	///     A headline for a news story.
	/// </summary>
	public class NewsDTO
	{
		/// <summary>
		///     The unique identifier for a news story.
		/// </summary>
		public int StoryId { get; set; }

		/// <summary>
		///     The news story headline.
		/// </summary>
		public string Headline { get; set; }

		/// <summary>
		///     The date on which the news story was published. Always in UTC.
		/// </summary>
		public string PublishDate { get; set; }

		public override string ToString()
		{
			return String.Join("/", StoryId, Headline, PublishDate);
		}
	}
}
