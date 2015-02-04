#region Usings

using System;

using NLog;
using NLog.Config;

#endregion

namespace StreamingServer.Common.Logging
{
	public class Logger : ILogger
	{
		private readonly NLog.Logger _log;

		public Logger(LoggingConfiguration loggingConfiguration)
		{
			LogManager.Configuration = loggingConfiguration;
			_log = LogManager.GetLogger("LightstreamerToSocketServer");
		}

		public void Error(string message)
		{
			_log.Error(message);
		}

		public void Error(Exception exception)
		{
			_log.Error(exception);
		}

		public void Error(string message, Exception exception)
		{
			_log.Error(message, exception);
		}

		public void Error(string message, params object[] args)
		{
			_log.Error(message, args);
		}

		public void Warn(string message)
		{
			_log.Warn(message);
		}

		public void Warn(Exception exception)
		{
			_log.Warn(exception);
		}

		public void Warn(string message, Exception exception)
		{
			_log.Warn(message, exception);
		}

		public void Warn(string message, params object[] args)
		{
			_log.Warn(message, args);
		}

		public void Info(string message)
		{
			_log.Info(message);
		}

		public void Info(Exception exception)
		{
			_log.Info(exception);
		}

		public void Info(string message, Exception exception)
		{
			_log.Info(message, exception);
		}

		public void Info(string message, params object[] args)
		{
			_log.Info(message, args);
		}

		public void Debug(string message)
		{
			_log.Debug(message);
		}
	}
}
