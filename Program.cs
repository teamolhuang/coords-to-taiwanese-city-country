using coords_to_taiwanese_city_country.Services;
using coords_to_taiwanese_city_country.Services.Abstracts;

namespace coords_to_taiwanese_city_country;

public class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(o =>
        {
            // 讓 swagger 包含 C# 的 XML documentation
            string xmlPath = Path.Combine(AppContext.BaseDirectory, "coords-to-taiwanese-city-country.xml");
            o.IncludeXmlComments(xmlPath, true);
        });

        builder.Services.AddScoped<ILocatingService, LocatingService>();
        
        WebApplication app = builder.Build();

        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseHttpsRedirection();

        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}