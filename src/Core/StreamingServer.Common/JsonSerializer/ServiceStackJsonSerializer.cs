#region Usings

using ServiceStack.Text;

#endregion

namespace StreamingServer.Common.JsonSerializer
{
	public class ServiceStackJsonSerializer : IJsonSerializer
	{
		public T DeserializeObject<T>(string str)
		{
			return new TypeSerializer<T>().DeserializeFromString(str);
		}
	}
}
