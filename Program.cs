using System.Text;
using coords_to_taiwanese_city_country.Entities;
using coords_to_taiwanese_city_country.Services;
using coords_to_taiwanese_city_country.Services.Abstracts;
using coords_to_taiwanese_city_country.Utilities;
using coords_to_taiwanese_city_country.Utilities.Abstracts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace coords_to_taiwanese_city_country;

/// <summary>
/// main class
/// </summary>
public class Program
{
    /// <summary>
    /// main
    /// </summary>
    public static async Task Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(o =>
        {
            // JWT 登入用功能
            o.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\"",
            });
            
            o.AddSecurityRequirement(new OpenApiSecurityRequirement { { new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } }, [] } });
            
            // 讓 swagger 包含 C# 的 XML documentation
            string xmlPath = Path.Combine(AppContext.BaseDirectory, "coords-to-taiwanese-city-country.xml");
            o.IncludeXmlComments(xmlPath, true);
        });

        builder.Services.AddScoped<ILocatingService, LocatingService>();
        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddSingleton<ICooldownService, CooldownService>();

        builder.Services.AddSingleton<IJwtTokenHelper, JwtTokenHelper>();
        builder.Services.AddSingleton<IJwtGuidHandler, JwtGuidHandler>();
        builder.Services.AddSingleton<IGmlHandler, GmlHandler>();

        // 在子目錄 db 底下建立 db file
        Directory.CreateDirectory("db");
        builder.Services.AddDbContext<DatabaseContext>(options =>
        {
            options.UseSqlite("Data Source=db\\database.db");
        });
        
        // 我們允許透過 docker-compose 的系統參數變動 appSettings
        builder.Configuration
            .AddEnvironmentVariables();
        
        // JWT handling
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(jwtOptions =>
            {
                string key = builder.Configuration.GetSection("Jwt").GetValue<string?>("SigningKey") ?? throw new NullReferenceException("JWT SigningKey is missing!");
                
                jwtOptions.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false,
                    ValidateActor = false,
                    ValidateIssuer = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                };

                jwtOptions.Events = new JwtBearerEvents
                {
                    OnTokenValidated = async context =>
                    {
                        bool isNotDeleted = await context
                            .HttpContext
                            .RequestServices
                            .GetRequiredService<IJwtGuidHandler>()
                            .ValidateGuidNotDeletedAsync(context);
                        
                        if (!isNotDeleted)
                            context.Fail("請重新登入");
                    }
                };
            });
        
        WebApplication app = builder.Build();
        
        // 先確保已建立並更新 db
        await using AsyncServiceScope scope = app.Services.CreateAsyncScope();
        await using DatabaseContext db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
        await db.Database.MigrateAsync();
        
        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();


        app.MapControllers();

        await app.RunAsync();
    }
}