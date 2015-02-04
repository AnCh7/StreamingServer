#region Usings

using System;
using System.IO;

using StreamingServer.SocketServer.Models;

#endregion

namespace StreamingServer.SocketServer.TransportLayer
{
	public class ResponseBuilder
	{
		private int _step;
		private MemoryStream _stream;
		private int _streamOffset;
		private readonly Action[] _steps;

		public ResponseBuilder()
		{
			_steps = new Action[] {AddContent, Cr, Lf};

			Response = new Response();
		}

		public Response Response { get; private set; }

		public int Process(ArraySegment<byte> segment)
		{
			_stream = new MemoryStream(segment.Array, segment.Offset, segment.Count, true, true);
			_streamOffset = segment.Offset;

			if (Response.Content != null)
			{
				ProcessSteps();
			}

			return (int)_stream.Position;
		}

		public void Reset()
		{
			Response = new Response();

			_stream = null;
			_streamOffset = 0;
			_step = 0;
		}

		private void ProcessSteps()
		{
			while (_stream.Position < _stream.Length && _step < _steps.Length)
			{
				_steps[_step]();
			}
		}

		public bool Completed
		{
			get { return _step >= _steps.Length; }
		}

		private void Cr()
		{
			Append(ByteCode.CarriageReturn);
		}

		private void Lf()
		{
			Append(ByteCode.LineFeed);
		}

		private void Append(byte character)
		{
			if (_stream.Position < _stream.Length)
			{
				_stream.WriteByte(character);
				_step++;
			}
		}

		private void AddContent()
		{
			var buffer = _stream.GetBuffer();

			var bytesRead = Response.Content.Read(buffer,
												  (int)_stream.Position + _streamOffset,
												  (int)(_stream.Length - _stream.Position));

			_stream.Position += bytesRead;

			if (Response.Content.Position >= Response.Content.Length)
			{
				Response.Content.Close();
				_step++;
			}
		}
	}
}
