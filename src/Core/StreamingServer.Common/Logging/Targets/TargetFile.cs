#region Usings

using System;
using System.IO;
using NLog;
using NLog.Config;
using NLog.Targets;

#endregion

namespace StreamingServer.Common.Logging.Targets
{
	public class TargetFile
	{
		/// <exception cref="UnauthorizedAccessException">The caller does not have the required permission. </exception>
		public LoggingConfiguration Initialize()
		{
			var config = new LoggingConfiguration();

			var target = new FileTarget
						 {
							 Name = "EventLogTarget",
							 FileName = Directory.GetCurrentDirectory() + "log.txt",
							 Layout = @"${longdate} ${message}"
						 };

			config.AddTarget("FileTarget", target);

			var rule = new LoggingRule("*", LogLevel.Debug, target);
			config.LoggingRules.Add(rule);

			return config;
		}
	}
}