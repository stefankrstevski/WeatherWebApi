using WeatherWebApi.DTOs;
using WeatherWebApi.Interfaces;
using WeatherWebApi.Responses;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Web;

public class WeatherService : IWeatherService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ApiSettings _apiSettings;
    private readonly ILogger<WeatherService> _logger;

    public WeatherService(IHttpClientFactory httpClientFactory, IOptions<ApiSettings> apiSettings, ILogger<WeatherService> logger) 
    {
        _httpClientFactory = httpClientFactory;
        _apiSettings = apiSettings.Value;
        _logger = logger;
    }


    public async Task<ApiResponse<List<DailySummaryDTO>>> GetWeatherForecastAsync(string city)
    {
        _logger.LogInformation($"Attempting to retrieve weather forecast for {city}.");
        try
        {
            var location = await GetCoordinatesAsync(city);
            var httpClient = _httpClientFactory.CreateClient();
            var response = await httpClient.GetStringAsync(ConstructUrl("data/2.5/forecast", new Dictionary<string, string>
            {
                ["lat"] = location.Lat.ToString(),
                ["lon"] = location.Lon.ToString(),
                ["appid"] = _apiSettings.ApiKey
            }));
            var weatherData = ProcessWeatherData(response);
            _logger.LogInformation($"Successfully retrieved weather forecast for {city}.");
            return new ApiResponse<List<DailySummaryDTO>>(weatherData, "Login successful.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to retrieve weather data for {city}.");
            return new ApiResponse<List<DailySummaryDTO>>(new List<string> { "Couldn't retrieve the data for the city." }, "Couldn't retrieve the data for the city.");
        }
    }

    private async Task<LocationDTO> GetCoordinatesAsync(string city)
    {
        _logger.LogInformation($"Getting coordinates for {city}.");
        var httpClient = _httpClientFactory.CreateClient();
        var response = await httpClient.GetStringAsync(ConstructUrl("geo/1.0/direct", new Dictionary<string, string>
        {
            ["q"] = city,
            ["limit"] = "1",
            ["appid"] = _apiSettings.ApiKey
        }));

        var locations = JsonSerializer.Deserialize<List<LocationDTO>>(response, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        if (locations == null || !locations.Any())
        {
            _logger.LogWarning($"No location found for {city}.");
            throw new Exception("Location not found");
        }

        _logger.LogInformation($"Coordinates retrieved for {city}.");
        return locations.First();
    }


    private List<DailySummaryDTO> ProcessWeatherData(string jsonString)
    {
        _logger.LogInformation("Processing weather data.");
        try
        {
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var weatherResponse = JsonSerializer.Deserialize<WeatherResponseDto>(jsonString, options) ??
                                  throw new InvalidOperationException("Failed to deserialize JSON.");

            if (weatherResponse == null) throw new InvalidOperationException("Failed to deserialize JSON.");

            // Mapping query to get the one object per day
            // Getting the highest temperature of the day at 15:00
            // and lowest at 03:00, and getting only one icon at 15:00 o clock.
            // if there isn't available data for the specific hour take the first available 
            // since the api may fail sometimes
            var dailySummaries = weatherResponse.Forecasts
            .GroupBy(forecast => DateTimeOffset.FromUnixTimeSeconds(forecast.DateTime).Date)
            .Select(group =>
            {
                var maxTempForecast = group.FirstOrDefault(f => DateTimeOffset.FromUnixTimeSeconds(f.DateTime).DateTime.Hour == 15)
                                      ?? group.OrderBy(f => f.DateTime).First();
                var minTempForecast = group.FirstOrDefault(f => DateTimeOffset.FromUnixTimeSeconds(f.DateTime).DateTime.Hour == 3)
                                      ?? group.OrderBy(f => f.DateTime).First();
                var iconForecast = group.FirstOrDefault(f => DateTimeOffset.FromUnixTimeSeconds(f.DateTime).DateTime.Hour == 15)
                                   ?? group.OrderBy(f => f.DateTime).First();

                return new DailySummaryDTO
                {
                    Date = group.Key,
                    MaxTemp = maxTempForecast.TemperatureInfo.TempMax - 273.15,
                    MinTemp = minTempForecast.TemperatureInfo.TempMin - 273.15,
                    WeatherIcon = iconForecast.WeatherConditions.Select(w => w.Icon).FirstOrDefault()
                };
            })
            .ToList();
            _logger.LogInformation("Weather data processed successfully.");
            return dailySummaries;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing weather data.");
            throw;
        }
    }
    private string ConstructUrl(string endpoint, Dictionary<string, string> queryParams)
    {
        var uriBuilder = new UriBuilder(new Uri(new Uri(_apiSettings.ApiUrl), endpoint));
        var query = HttpUtility.ParseQueryString(uriBuilder.Query);
        foreach (var param in queryParams)
        {
            query[param.Key] = param.Value;
        }
        uriBuilder.Query = query.ToString();
        return uriBuilder.ToString();
    }

}
