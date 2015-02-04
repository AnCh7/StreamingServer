#region Usings

using System;
using System.Configuration;

#endregion

namespace StreamingServer.SocketServer.TestClient
{
	public class Settings
	{
		public static string Url()
		{
			var defaultValue = "127.0.0.1";

			try
			{
				var url = ConfigurationManager.AppSettings["Url"];

				if (String.IsNullOrWhiteSpace(url))
				{
					defaultValue = url;
				}
			}
			catch (Exception ex)
			{
				// ignored
			}

			return defaultValue;
		}

		public static int BufferSize()
		{
			var defaultValue = 64;

			try
			{
				var bufferSize = ConfigurationManager.AppSettings["BufferSize"];
				defaultValue = Convert.ToInt32(bufferSize);
			}
			catch (Exception ex)
			{
				// ignored
			}

			return defaultValue;
		}

		public static int Port()
		{
			var defaultValue = 5512;

			try
			{
				var port = ConfigurationManager.AppSettings["Port"];
				defaultValue = Convert.ToInt32(port);
			}
			catch (Exception ex)
			{
				// ignored
			}

			return defaultValue;
		}
	}
}
