using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using WebApplication1.Dto;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries =
        [
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        ];

        private static List<WeatherForecast> listWeather = new List<WeatherForecast>();
            

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpGet("{id}", Name = "GetForecast")]
        public ActionResult<WeatherForecast> GetById(int id) {
            if (id <= 0)
            {
                return BadRequest("It must be greater than 0.");
            }

            var weather = new WeatherForecastDto()
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(id)),
                TemperatureC = Random.Shared.Next(-20, 55),
            };

            return Ok(weather);

        }

        [HttpPost("add")]
        public ActionResult<WeatherForecastDto> AddWeather([FromBody] WeatherForecast weatherForecast)
        {

            listWeather.Add(weatherForecast);

            var weatherForecastDto = new WeatherForecastDto(weatherForecast.Date,
                weatherForecast.TemperatureC);
        
           return Ok(weatherForecastDto);
        }

        [HttpPut("edit/{id}")]
        public ActionResult<WeatherForecastDto> EditWeather(int id, [FromBody] WeatherForecast weatherForecast)
        {

            listWeather.Add(weatherForecast);

            var weatherForecastDto = new WeatherForecastDto(weatherForecast.Date,
                weatherForecast.TemperatureC);

            return Ok(weatherForecastDto);
        }
    }
}
