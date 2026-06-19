using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebApplication1.Data;
using WebApplication1.Dto;
using WebApplication1.Models;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PetsController : ControllerBase
    {
        private readonly HomeDbContext context;
        private readonly Cloudinary cloudinary;

        public PetsController(HomeDbContext context, Cloudinary cloudinary)
        {
            this.context = context;
            this.cloudinary = cloudinary;
        }

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
                imageUrl = pet.image,
            };

            return Ok(pet1);
        }

        //[Authorize]
        [HttpGet("findByUser/{id}")]
        public ActionResult<PetsDto> FindByUser(int id)
        {
            //var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            //var castUserId = int.Parse(userId);

            //if (string.IsNullOrEmpty(userId))
            //{
            //    return Unauthorized("User doesn't exist.");
            //}

            var pets = context.pets.Where(p => p.toBabysit == id).Select(p => new PetsDto
            {
                name = p.name,
                age = p.age,
                phone = p.phone,
                type = p.type,
                imageUrl = p.image,
            }).ToList();

            if (pets.Count == 0)
            {
                return NotFound("You did not add any pets.");
            }

            return Ok(pets);
        }

        [HttpGet("findByUserAndType/{id}/{type}")]
        public ActionResult<PetsDto> FindByUserAndType(int id, string type)
        {
            //var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            //var castUserId = int.Parse(userId);

            //if (string.IsNullOrEmpty(userId))
            //{
            //    return Unauthorized("User doesn't exist.");
            //}

            var pets = context.pets.Where(p => p.toBabysit == id && p.type == type).Select(p => new PetsDto
            {
                name = p.name,
                age = p.age,
                phone = p.phone,
                type = p.type,
                imageUrl = p.image,
            }).ToList();

            if (pets.Count == 0)
            {
                return NotFound("You did not add any pets.");
            }

            return Ok(pets);
        }

        //[Authorize]
        [HttpGet("findByToBabysitUser/{id}")]
        public ActionResult<PetsDto> FindByToBabysitUser(int id)
        {
            //var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            //var castUserId = int.Parse(userId);


            //if (string.IsNullOrEmpty(userId))
            //{
            //    return Unauthorized("User doesn't exist.");
            //}

            var pets = context.pets.Where(p => p.userId == id && p.toBabysit != id).Select(p => new PetsDto
            {
                name = p.name,
                age = p.age,
                phone = p.phone,
                type = p.type,
                imageUrl = p.image,
            }).ToList();

            if (pets.Count == 0)
            {
                return NotFound("You do not have any pets to babysit.");
            }

            return Ok(pets);
        }

        //[Authorize]
        [HttpGet("findByToBabysitUserAndType/{id}/{type}")]
        public ActionResult<PetsDto> FindByToBabysitUserAndType(int id, string type)
        {
            //var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            //var castUserId = int.Parse(userId);


            //if (string.IsNullOrEmpty(userId))
            //{
            //    return Unauthorized("User doesn't exist.");
            //}

            var pets = context.pets.Where(p => p.userId == id && p.toBabysit != id && p.type == type).Select(p => new PetsDto
            {
                name = p.name,
                age = p.age,
                phone = p.phone,
                type = p.type,
                imageUrl = p.image,
            }).ToList();

            if (pets.Count == 0)
            {
                return NotFound("You do not have any pets to babysit.");
            }

            return Ok(pets);
        }

        [Authorize]
        [HttpPost("add")]
        public ActionResult<PetsDto> AddPets([FromForm] PetsDto pets)
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

            string? imageUrl = null;

            if (pets.image != null && pets.image.Length > 0)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };

                var extension = Path.GetExtension(pets.image.FileName).ToLower();

                if (!allowedExtensions.Contains(extension))
                {
                    return BadRequest("Invalid image type.");
                }

                var uploadsFolder = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "uploads",
                    "pets"
                 );

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var fileName = $"{Guid.NewGuid()}{extension}";

                var filePath = Path.Combine (uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    pets.image.CopyTo(stream);
                }

                imageUrl = $"/uploads/pets/{fileName}";
            }


            var pets1 = new Pets()
            {
                name = pets.name,
                age = pets.age,
                phone = pets.phone,
                type = pets.type,
                image = imageUrl,
                toBabysit = castUserId
            };

            context.pets.Add(pets1);
            context.SaveChanges();

            return Ok(pets);
        }

        [Authorize]
        [HttpPut("edit/{name}/{age}/{phone}/{type}")]
        public ActionResult<PetsDto> EditPets(string name, int age, string phone, string type, [FromBody] PetsDto pets)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var castUserId = int.Parse(userId);

            var existingPets = context.pets.FirstOrDefault(p => p.name == name && p.age == age && p.phone == phone && p.type == type);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User doesn't exist.");
            }

            if(existingPets == null)
            {
                return BadRequest("Pet doesn't exist.");
            }

            existingPets.name = pets.name;
            existingPets.age = pets.age;
            existingPets.phone = pets.phone;
            existingPets.type = pets.type;
            existingPets.userId = pets.userId;
                
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

        [Authorize]
        [HttpDelete("delete/{name}/{age}/{phone}/{type}")]
        public ActionResult<PetsDto> DeletePets(string name, int age, string phone, string type)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var castUserId = int.Parse(userId);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User doesn't exist.");
            }

            var existingPets = context.pets.FirstOrDefault(p => p.name == name && p.age == age && p.phone == phone && p.type == type);

            if (existingPets == null)
            {
                return BadRequest("Pet doesn't exist.");
            }

            context.pets.Remove(existingPets);

            context.SaveChanges();

            return Ok("Pet with name " + existingPets.name + " deleted successfully.");
        }
    }
}
