using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using coords_to_taiwanese_city_country.Utilities.Abstracts;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace coords_to_taiwanese_city_country.Utilities;

/// <inheritdoc />
public class JwtTokenHelper : IJwtTokenHelper
{
    /// <summary>
    /// JWT 有效分鐘數
    /// </summary>
    private int ExpireMinutes { get; init; }
    
    /// <summary>
    /// 取得實例。
    /// </summary>
    public JwtTokenHelper(IConfiguration configuration)
    {
        ExpireMinutes = configuration.GetSection("Jwt").GetValue<int?>("ExpireMinutes") ?? 60;
    }
    
    /// <inheritdoc />
    public string GenerateToken(Guid userId, string privateKey)
    {
        SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(privateKey));
        DateTime expiration = DateTime.UtcNow.AddMinutes(ExpireMinutes);
        
        JsonWebTokenHandler handler = new();

        string? token = handler.CreateToken(new SecurityTokenDescriptor
        {
            Expires = expiration,
            Claims = new Dictionary<string, object>()
            {
                { ClaimTypes.NameIdentifier, userId.ToString() }  
            },
            SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature)
        });
        
        return token ?? throw new NullReferenceException("CreateToken failed");
    }

    /// <inheritdoc />
    public TimeSpan GetExpirationSpan()
    {
        return TimeSpan.FromMinutes(ExpireMinutes);
    }
}