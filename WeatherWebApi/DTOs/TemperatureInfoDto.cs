using System.Text.Json.Serialization;

namespace WeatherWebApi.DTOs
{
    public class TemperatureInfoDto
    {
        [JsonPropertyName("temp_max")]
        public double TempMax { get; set; }

        [JsonPropertyName("temp_min")]
        public double TempMin { get; set; }
    }
}
