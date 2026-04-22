using Microsoft.EntityFrameworkCore;
using WebApplication1.Dto;
using WebApplication1.Models;

namespace WebApplication1.Data
{
    public class HomeDbContext(DbContextOptions<HomeDbContext> options) : DbContext(options)
    {
        public DbSet<Home> homes { get; set; }

        public DbSet<WeatherForecast> weatherForecasts { get; set; }

        public DbSet<User> users { get; set; }

        public DbSet<Pets> pets { get; set; }
    }
}
