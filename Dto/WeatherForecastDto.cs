namespace WebApplication1.Dto
{
    public class WeatherForecastDto
    {
        public DateOnly Date { get; set; }
        public int TemperatureC { get; set; }

        public WeatherForecastDto() { }

        public WeatherForecastDto(DateOnly date, int temperatureC)
        {
            Date = date;
            TemperatureC = temperatureC;
        }
    }
}
