namespace StreamingServer.Common.JsonSerializer
{
	public interface IJsonSerializer
	{
		T DeserializeObject<T>(string str);
	}
}
