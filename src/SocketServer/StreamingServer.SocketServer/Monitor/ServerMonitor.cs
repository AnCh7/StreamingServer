#region Usings

using System.Threading;

#endregion

namespace StreamingServer.SocketServer.Monitor
{
	public class ServerMonitor
	{
		private int _acceptsCompleted;
		private int _acceptsPosted;

		private int _clientsClosed;

		private int _receivesCompleted;
		private int _receivesPosted;

		private int _sendsCompleted;
		private int _sendsPosted;

		public string ServerUrl { get; set; }

		public int AcceptsPosted
		{
			get { return Interlocked.CompareExchange(ref _acceptsPosted, 0, 0); }
		}

		public int AcceptsCompleted
		{
			get { return Interlocked.CompareExchange(ref _acceptsCompleted, 0, 0); }
		}

		public int ReceivesPosted
		{
			get { return Interlocked.CompareExchange(ref _receivesPosted, 0, 0); }
		}

		public int ReceivesCompleted
		{
			get { return Interlocked.CompareExchange(ref _receivesCompleted, 0, 0); }
		}

		public int SendsPosted
		{
			get { return Interlocked.CompareExchange(ref _sendsPosted, 0, 0); }
		}

		public int SendsCompleted
		{
			get { return Interlocked.CompareExchange(ref _sendsCompleted, 0, 0); }
		}

		public int ClientsClosed
		{
			get { return Interlocked.CompareExchange(ref _clientsClosed, 0, 0); }
		}

		public void StartAccept()
		{
			Interlocked.Increment(ref _acceptsPosted);
		}

		public void ProcessAccept()
		{
			Interlocked.Increment(ref _acceptsCompleted);
		}

		public void StartReceive()
		{
			Interlocked.Increment(ref _receivesPosted);
		}

		public void ProcessReceive()
		{
			Interlocked.Increment(ref _receivesCompleted);
		}

		public void StartSend()
		{
			Interlocked.Increment(ref _sendsPosted);
		}

		public void ProcessSend()
		{
			Interlocked.Increment(ref _sendsCompleted);
		}

		public void CloseClient()
		{
			Interlocked.Increment(ref _clientsClosed);
		}
	}
}
