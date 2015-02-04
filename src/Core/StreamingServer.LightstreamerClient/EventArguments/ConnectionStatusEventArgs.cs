#region Usings

using System;

using StreamingServer.LightstreamerClient.Enums;

#endregion

namespace StreamingServer.LightstreamerClient.EventArguments
{
	public class ConnectionStatusEventArgs : EventArgs
	{
		public string Message { get; set; }
		public ConnectionStatus Status { get; set; }

		public ConnectionStatusEventArgs(string message, ConnectionStatus status)
		{
			Message = message;
			Status = status;
		}

		public override string ToString()
		{
			return string.Format("Message: {0}, Status: {1}", Message, Status);
		}
	}
}
