using System.Text.Json.Serialization;

namespace WeatherWebApi.DTOs
{
    public class WeatherResponseDto
    {
        [JsonPropertyName("cod")]
        public string Cod { get; set; } = string.Empty;

        [JsonPropertyName("message")]
        public int Message { get; set; }

        [JsonPropertyName("cnt")]
        public int Count { get; set; }

        [JsonPropertyName("list")]
        public List<WeatherForecastDto> Forecasts { get; set; } = new List<WeatherForecastDto>();
    }
}
