#region Usings

using System;

#endregion

namespace StreamingServer.Common.Logging
{
	public interface ILogger
	{
		void Error(string message);

		void Error(Exception exception);

		void Error(string message, Exception exception);

		void Error(string message, params object[] args);

		void Warn(string message);

		void Warn(Exception exception);

		void Warn(string message, Exception exception);

		void Warn(string message, params object[] args);

		void Info(string message);

		void Info(Exception exception);

		void Info(string message, Exception exception);

		void Info(string message, params object[] args);

		void Debug(string message);
	}
}
