namespace Weather.Application.Interfaces
{
    public interface IWeatherService
    {
        Task<string> GetWeatherDataAsync(string city, string countryCode);
    }
}
