#region Usings

using System;
using System.IO;
using System.Text;

using StreamingServer.SocketServer.Models;

#endregion

namespace StreamingServer.SocketServer.TransportLayer
{
	public class RequestBuilder
	{
		private int _currentByte;
		private int _step;
		private Stream _stream;
		private readonly Action[] _steps;

		public RequestBuilder()
		{
			_steps = new Action[] { GetUserName, Dot, GetSessionToken, Dot, GetRequestType, Dot, GetParameters, Cr, Lf };

			Request = new Request();
		}

		public Request Request { get; private set; }

		public bool Process(ArraySegment<byte> segment)
		{
			_stream = new MemoryStream(segment.Array, segment.Offset, segment.Count);
			_currentByte = _stream.ReadByte();

			ProcessSteps();

			return _step >= _steps.Length;
		}

		public void Reset()
		{
			Request = new Request();

			_stream = null;
			_step = 0;
			_currentByte = 0;
		}

		private void ProcessSteps()
		{
			while (_currentByte >= 0 && _step < _steps.Length)
			{
				_steps[_step]();
			}
		}

		private void GetUserName()
		{
			Request.UserName += Token(ByteCode.Dot);
		}

		private void GetSessionToken()
		{
			Request.SessionToken += Token(ByteCode.Dot);
		}

		private void GetRequestType()
		{
			Request.RequestType += Token(ByteCode.Dot);
		}

		private void GetParameters()
		{
			Request.Parameters += Token(ByteCode.CarriageReturn);
		}

		private void Cr()
		{
			Const(ByteCode.CarriageReturn);
		}

		private void Lf()
		{
			Const(ByteCode.LineFeed);
		}

		private void Dot()
		{
			Const(ByteCode.Dot);
		}

		private void Const(int character)
		{
			if (_currentByte != character)
			{
				throw new ApplicationException(string.Format("Failed parsing character.\r\n\tExpected: '{0}'\r\n\tActual: '{1}'",
															 (char)character,
															 (char)_currentByte));
			}

			_currentByte = _stream.ReadByte();

			_step++;
		}

		private string Token(int delimiter)
		{
			var stringBuilder = new StringBuilder();

			while (_currentByte != delimiter && _currentByte >= 0)
			{
				stringBuilder.Append((char)_currentByte);
				_currentByte = _stream.ReadByte();
			}

			if (_currentByte == delimiter)
			{
				_step++;
			}

			return stringBuilder.ToString();
		}
	}
}
