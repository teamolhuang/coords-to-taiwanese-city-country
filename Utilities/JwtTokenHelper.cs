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
    /// <inheritdoc />
    public string GenerateToken(Guid userId, DateTime expiration, string privateKey)
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