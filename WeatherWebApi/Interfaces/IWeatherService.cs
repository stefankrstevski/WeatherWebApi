using WeatherWebApi.DTOs;
using WeatherWebApi.Responses;

namespace WeatherWebApi.Interfaces
{
    public interface IWeatherService
    {
        Task<ApiResponse<List<DailySummaryDTO>>> GetWeatherForecastAsync(string cityName);
    }
}
