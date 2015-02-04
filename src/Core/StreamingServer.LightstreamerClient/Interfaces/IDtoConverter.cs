namespace StreamingServer.LightstreamerClient.Interfaces
{
	public interface IDtoConverter<out T>
	{
		T Convert(object data);
	}
}
