namespace WeatherWebApi.DTOs
{
    public class DailySummaryDTO
    {
        public DateTime Date { get; set; }
        public double MaxTemp { get; set; }
        public double MinTemp { get; set; }
        public string WeatherIcon { get; set; } = string.Empty;
    }
}
