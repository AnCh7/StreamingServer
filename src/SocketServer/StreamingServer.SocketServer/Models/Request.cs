#region Usings

using System;

#endregion

namespace StreamingServer.SocketServer.Models
{
	public class Request : IEquatable<Request>
	{
		public string UserName { get; set; }

		public string SessionToken { get; set; }

		public string RequestType { get; set; }

		public string Parameters { get; set; }

		public override string ToString()
		{
			return String.Join(".", UserName, SessionToken, RequestType, Parameters);
		}

		public bool Equals(Request other)
		{
			if (ReferenceEquals(null, other))
			{
				return false;
			}
			if (ReferenceEquals(this, other))
			{
				return true;
			}

			return string.Equals(UserName, other.UserName) &&
				   string.Equals(SessionToken, other.SessionToken) &&
				   string.Equals(RequestType, other.RequestType) &&
				   string.Equals(Parameters, other.Parameters);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
			{
				return false;
			}
			if (ReferenceEquals(this, obj))
			{
				return true;
			}

			if (obj.GetType() != GetType())
			{
				return false;
			}

			return Equals((Request)obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = (UserName != null ? UserName.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (SessionToken != null ? SessionToken.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (RequestType != null ? RequestType.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (Parameters != null ? Parameters.GetHashCode() : 0);
				return hashCode;
			}
		}
	}
}
