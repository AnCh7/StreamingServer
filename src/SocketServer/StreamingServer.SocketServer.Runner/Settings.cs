#region Usings

using System;
using System.Configuration;

#endregion

namespace StreamingServer.SocketServer.Runner
{
	public class Settings
	{
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

		public static int Backlog()
		{
			var defaultValue = 100;

			try
			{
				var backlog = ConfigurationManager.AppSettings["Backlog"];
				defaultValue = Convert.ToInt32(backlog);
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
