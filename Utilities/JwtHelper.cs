using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace coords_to_taiwanese_city_country.Utilities;

/// <summary>
/// JWT Token 發行相關的工具
/// </summary>
public class JwtHelper
{
    /// <summary>
    /// 發行 JWT
    /// </summary>
    public static string GenerateToken(Guid userId, DateTime expiration, string privateKey)
    {
        SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(privateKey));
        
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
}