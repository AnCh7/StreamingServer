#region Usings

using System;

#endregion

namespace StreamingServer.LightstreamerClient.EventArguments
{
	public class MessageEventArgs<T> : EventArgs
	{
		public string DataAdapter { get; set; }
		public int Phase { get; set; }
		public string Topic { get; set; }
		public T Data { get; set; }

		public MessageEventArgs(string dataAdapter, string topic, T messageData, int phase)
		{
			DataAdapter = dataAdapter;
			Topic = topic;
			Data = messageData;
			Phase = phase;
		}

		public override string ToString()
		{
			return string.Format("DataAdapter: {0}, Phase: {1}, Topic: {2}, Data: {3}", DataAdapter, Phase, Topic, Data);
		}
	}
}
