
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace WebApplication1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: MyAllowSpecificOrigins,
                                  builder =>
                                  {
                                      builder.WithOrigins("http://localhost:3000", "https://pet-app-frontend-eta.vercel.app", "https://pet-app-frontend-709ahqrh2-amels-projects-f7598538.vercel.app")
                                      .AllowAnyHeader()
                                      .AllowAnyMethod()
                                      .AllowCredentials();
                                  });
            });
            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            builder.Services.AddDbContext<HomeDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

            var jwtKey = builder.Configuration["Jwt:Key"];
            var jwtIssuer = builder.Configuration["Jwt:Issuer"];
            var jwtAudience = builder.Configuration["Jwt:Audience"];

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey!))
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var token = context.Request.Cookies["authToken"];

                if (!string.IsNullOrEmpty(token))
                {
                    context.Token = token;
                }

                return Task.CompletedTask;
            }
        };
    })
    .AddCookie("External")
    .AddGoogle(options =>
    {
        options.SignInScheme = "External";
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"]!;
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]!;
    });

            builder.Services.AddAuthorization();

            var app = builder.Build();

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            Console.WriteLine("Google ClientId loaded: " + !string.IsNullOrEmpty(builder.Configuration["Authentication:Google:ClientId"]));
            Console.WriteLine("Google ClientSecret loaded: " + !string.IsNullOrEmpty(builder.Configuration["Authentication:Google:ClientSecret"]));
            Console.WriteLine("JWT Key loaded: " + !string.IsNullOrEmpty(builder.Configuration["Jwt:Key"]));
            Console.WriteLine("Connection string loaded: " + !string.IsNullOrEmpty(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseCors(MyAllowSpecificOrigins);

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
