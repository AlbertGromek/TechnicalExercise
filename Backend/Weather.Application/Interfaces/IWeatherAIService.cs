using Weather.Domain.Models;

namespace Weather.Application.Interfaces
{
    public interface IWeatherAIService
    {
        Task<string> GetWhatToWearAsync(WeatherAIRequest weatherAIRequest);
        Task<string> GetDayRecommendationsAsync(WeatherAIRequest weatherAIRequest);
    }
}