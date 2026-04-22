using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApplication1.Data;
using WebApplication1.Dto;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController(HomeDbContext context) : ControllerBase
    {
        private readonly PasswordHasher<User> passwordHasher = new();

        [HttpPost("register")]
        public ActionResult Register([FromBody] RegisterDto registerDto)
        {
            var existingUser = context.users.FirstOrDefault(u => u.email == registerDto.Email);

            if(existingUser != null)
            {
                return BadRequest("User already exists!");
            }

            var user = new User
            {
                name = registerDto.Name,
                surname = registerDto.Surname,
                email = registerDto.Email,
                //phone = registerDto.Phone
            };

            //user.name = registerDto.Name;

            //user.surname = registerDto.Surname;



            user.password = passwordHasher.HashPassword(user, registerDto.Password);

            context.users.Add(user);

            context.SaveChanges();

            return Ok("User registered successfully.");
        }

        [HttpPost("login")]
        public ActionResult Login([FromBody] LoginDto loginDto){
            var user = context.users.FirstOrDefault(u => u.email == loginDto.Email);

            if (user == null) {
                return Unauthorized("Invalid email or password.");
            }

            var result = passwordHasher.VerifyHashedPassword(user, user.password, loginDto.Password);

            if(result == PasswordVerificationResult.Failed)
            {
                return Unauthorized("Invalid email or password");
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.id.ToString()),
                new Claim(ClaimTypes.Email, user.email),
                new Claim(ClaimTypes.Role, user.role)
            };

            var key = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(HttpContext.RequestServices
                    .GetRequiredService<IConfiguration>()["Jwt:Key"]!)
                );

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                    issuer: HttpContext.RequestServices.GetRequiredService<IConfiguration>()["Jwt:Issuer"],
                    audience: HttpContext.RequestServices.GetRequiredService<IConfiguration>()["Jwt:Audience"],
                    claims: claims,
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials: creds
                );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            Response.Cookies.Append("authToken", jwt, new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Lax,
                Expires = DateTimeOffset.UtcNow.AddDays(1)
            });

            return Ok(new {email = user.email});
        }

        [HttpPost("logout")]
        public ActionResult Logout()
        {
            Response.Cookies.Delete("authToken");

            return Ok(new { message = "User logged out successfully." });
        }

        [HttpGet("google-login")]
        public IActionResult GoogleLogin()
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action(nameof(GoogleResponse))
            };

            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet("google-response")]
        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync("External");

            if (!result.Succeeded || result.Principal == null)
            {
                return Unauthorized("Google authentication failed.");
            }

            var email = result.Principal.FindFirst(ClaimTypes.Email)?.Value;
            var googleId = result.Principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var user = context.users.FirstOrDefault(u => u.email == email);

            if (user == null)
            {
                user = new User
                {
                    email = email,
                    role = "User",
                    GoogleId = googleId,
                    AuthProvider = "Google",
                    password = string.Empty
                };

                context.Add(user);

                context.SaveChanges();
            }
            else
            {
                if (string.IsNullOrEmpty(googleId))
                {
                    user.GoogleId = googleId;
                    user.AuthProvider = "Google";
                    context.SaveChanges();
                }
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.id.ToString()),
                new Claim(ClaimTypes.Email, user.email),
                new Claim(ClaimTypes.Role, user.role)
            };

            var key = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(HttpContext.RequestServices
                    .GetRequiredService<IConfiguration>()["Jwt:Key"]!)
                );

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                    issuer: HttpContext.RequestServices.GetRequiredService<IConfiguration>()["Jwt:Issuer"],
                    audience: HttpContext.RequestServices.GetRequiredService<IConfiguration>()["Jwt:Audience"],
                    claims: claims,
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials: creds
                );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            Response.Cookies.Append("authToken", jwt, new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Lax,
                Expires = DateTimeOffset.UtcNow.AddDays(1)
            });

            await HttpContext.SignOutAsync("External");

            return Redirect("http://localhost:3000/");
            //return Ok(new { token = jwt, email = user.email });
            }

        [Authorize]
        [HttpGet("me")]
        public ActionResult GetMe()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            return Ok(new
            {
                userId,
                email,
                role
            });
        }
    }
    }
