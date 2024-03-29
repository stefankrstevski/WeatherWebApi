using System.Text.Json.Serialization;

namespace WeatherWebApi.DTOs
{
    public class WeatherForecastDto
    {
        [JsonPropertyName("dt")]
        public long DateTime { get; set; }

        [JsonPropertyName("main")]
        public TemperatureInfoDto TemperatureInfo { get; set; } = new TemperatureInfoDto();

        [JsonPropertyName("weather")]
        public List<WeatherConditionDto> WeatherConditions { get; set; } = new List<WeatherConditionDto>();
    }
}
