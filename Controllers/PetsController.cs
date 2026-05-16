using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApplication1.Data;
using WebApplication1.Dto;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PetsController(HomeDbContext context) : ControllerBase
    {
        [HttpGet]
        public ActionResult<List<PetsDto>> GetPets()
        {
            var pets = context.pets.ToList();

            if(pets.Count == 0)
            {
                return BadRequest("No Pets available at the moment.");
            }

            return Ok(pets);
        }

        [HttpGet("byType/{type}")]
        public ActionResult<List<PetsDto>> GetPetsByType(string type)
        {
            var cats = context.pets.Where(pet => pet.type == type).ToList();

            if(cats.Count == 0)
            {
                return BadRequest("No Cats found.");
            }

            return Ok(cats);
        }

        [HttpGet("byPet/{name}")]
        public ActionResult<PetsDto> FindByName(string name)
        {
            var pet = context.pets.FirstOrDefault(p => p.name == name);

            if (pet == null)
            {
                return BadRequest("Pet not found.");
            }

            var pet1 = new PetsDto()
            {
                name = pet.name,
                age = pet.age,
                phone = pet.phone,
                type = pet.type,
                image = pet.image
            };

            return Ok(pet1);
        }

        [Authorize]
        [HttpGet("findByUser/{id}")]
        public ActionResult<PetsDto> FindByUser()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var castUserId = int.Parse(userId);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User doesn't exist.");
            }

            var pets = context.pets.Where(p => p.toBabysit == castUserId).Select(p => new PetsDto
            {
                name = p.name,
                age = p.age,
                phone = p.phone,
                type = p.type,
                image = p.image,
            }).ToList();

            if (pets.Count == 0)
            {
                return NotFound("You did not add any pets.");
            }

            return Ok(pets);
        }

        [Authorize]
        [HttpGet("findByToBabysitUser/{id}")]
        public ActionResult<PetsDto> FindByToBabysitUser()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var castUserId = int.Parse(userId);


            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User doesn't exist.");
            }

            var pets = context.pets.Where(p => p.userId == castUserId && p.toBabysit == null).Select(p => new PetsDto
            {
                name = p.name,
                age = p.age,
                phone = p.phone,
                type = p.type,
                image = p.image,
            }).ToList();

            if (pets.Count == 0)
            {
                return NotFound("You do not have any pets to babysit.");
            }

            return Ok(pets);
        }

        [Authorize]
        [HttpPost("add")]
        public ActionResult<PetsDto> AddPets([FromBody] PetsDto pets)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var castUserId = int.Parse(userId);
            //var existingPet = context.pets.FirstOrDefault(ep => ep.name == pets.name);
            var existingPet = context.pets.Where(ep => ep.name == pets.name).ToList();

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User doesn't exist.");
            }

            foreach (var pet in existingPet)
            {
                if (pet != null &&
                    pet.type == pets.type &&
                    pet.age == pets.age &&
                    pet.phone == pets.phone)
                {
                    return BadRequest("Pet with that information already exists.");
                }
            }

            var pets1 = new Pets()
            {
                name = pets.name,
                age = pets.age,
                phone = pets.phone,
                type = pets.type,
                image = pets.image,
                toBabysit = castUserId
            };

            context.pets.Add(pets1);
            context.SaveChanges();

            return Ok(pets);
        }

        [Authorize]
        [HttpPut("edit/{name}")]
        public ActionResult<PetsDto> EditPets(string name, [FromBody] PetsDto pets)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var castUserId = int.Parse(userId);

            var existingPets = context.pets.FirstOrDefault(p => p.name == name);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User doesn't exist.");
            }

            if(existingPets == null)
            {
                return BadRequest("Pet doesn't exists.");
            }

            existingPets.name = pets.name;
            existingPets.age = pets.age;
            existingPets.phone = pets.phone;
            existingPets.type = pets.type;
            existingPets.userId = castUserId;
                
            //var pets1 = new Pets()
            //{
            //    name = pets.name,
            //    age = pets.age,
            //    phone = pets.phone,
            //    type = pets.type,
            //    image = pets.image,
            //    userId = castUserId
            //};

            //context.pets.Add(existingPets);

            context.SaveChanges();

            var pets2 = new PetsDto()
            {
                name = pets.name,
                age = pets.age,
                phone = pets.phone,
                type = pets.type,
                image = pets.image
            };

            return Ok(existingPets);
        }
    }
}
