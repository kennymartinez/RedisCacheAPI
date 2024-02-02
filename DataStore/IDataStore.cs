namespace RedisCacheApi.DataStore;

public interface IDataStore
{
    Task<WeatherForecast[]> GetDataAsync();
}