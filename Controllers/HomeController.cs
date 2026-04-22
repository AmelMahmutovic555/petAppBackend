using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using WebApplication1.Data;
using WebApplication1.Dto;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController(HomeDbContext context) : ControllerBase
    {
        //public static List<Home> homes = new List<Home>()
        //{ 
        //  new Home(1, "Mansion", 2000000),
        //  new Home(2, "House", 500000),
        //  new Home(3, "Cabin", 350000)
        //};

        [HttpGet]
        public ActionResult<List<HomeDto>> FindHouses()
        {
            var homes = context.homes.ToList();
            if (homes.Count == 0)
            {
                return NotFound("House Empty!");
            }

            var homeDtos = homes
                .Select(h => new HomeDto(h.type, h.price))
                .ToList();

            return Ok(homeDtos);
        }

        [HttpGet("{id}", Name = "getHomeById")]
        public ActionResult<HomeDto> HomeById(int id) {
            //Home home1 = homes.Find(home => home.id == id);
            var home1 = context.Find<Home>(id);

            if (home1 == null)
            {
                return NotFound("Home doesn't exist!");
            }

            HomeDto homeDto = new HomeDto(home1.type, home1.price);

            //context.SaveChanges();

            return Ok(homeDto);
        }

        [Authorize]
        [HttpPost("add")]
        public ActionResult<HomeDto> AddHome([FromBody] Home home) {

            //Home home1 = homes.Find(h => h.id == home.id);

            //var home2 =
            //context.Find<Home>(home.id);
            //var home3 = context.Find<HomeDto>(home.price);

            var home123 = context.homes.ToList();

            foreach(var h in home123)
            {   
                if(h.type == home.type && h.price == home.price)
                {
                    return BadRequest("You already added the house with that id.");
                }
            }

            //homes.Add(home);

            var home1 = new Home(home.type, home.price);
            //{
            //    type = home.type,
            //    price = home.price
            //};

            context.Add(home1);

            HomeDto homeDto = new HomeDto(home.type, home.price);

            context.SaveChanges();

            return Ok(homeDto);
        }

        [HttpPut("edit/{id}")]
        public ActionResult<HomeDto> EditHome(int id, [FromBody] Home home)
        {
            var home1 = context.Find<Home>(id);

            if (home1 == null)
            {
                return NotFound();
            }


            //home1.id = home.id;
            home1.price = home.price;
            home1.type = home.type;

            HomeDto homeDto = new HomeDto(home1.type, home1.price);

            context.SaveChanges();

            return Ok(homeDto);
        }

        [HttpDelete("delete/{id}")]
        public ActionResult<HomeDto> DeleteHome(int id)
        {
            //Home home1 = homes.Find(h => h.id == id);
            var home1 = context.Find<Home>(id);


            if (home1 == null)
            {
                return NotFound();
            }

            context.Remove(home1);

            context.SaveChanges();

            return NoContent();
        }
    }
}
