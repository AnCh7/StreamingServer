#region Usings

using NLog;
using NLog.Config;
using NLog.Targets;

#endregion

namespace StreamingServer.Common.Logging.Targets
{
	public sealed class TargetWindowsEvents
	{
		public LoggingConfiguration Initialize()
		{
			var config = new LoggingConfiguration();

			var target = new EventLogTarget
						 {
							 Source = "LightstreamerToSocketServer",
							 Log = "Application",
							 MachineName = ".",
							 Name = "EventLogTarget",
							 Layout = "${message}"
						 };

			config.AddTarget("EventLogTarget", target);

			var rule = new LoggingRule("*", LogLevel.Debug, target);
			config.LoggingRules.Add(rule);

			return config;
		}
	}
}