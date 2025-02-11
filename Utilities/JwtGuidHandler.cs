using System.Security.Claims;
using coords_to_taiwanese_city_country.Entities;
using coords_to_taiwanese_city_country.Utilities.Abstracts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;

namespace coords_to_taiwanese_city_country.Utilities;

/// <inheritdoc />
public class JwtGuidHandler : IJwtGuidHandler
{
    /// <summary>
    /// 驗證 claim 中的 GUID 尚未在本系統被刪除
    /// </summary>
    public async Task<bool> ValidateGuidNotDeletedAsync(TokenValidatedContext context)
    {
        Claim[]? principalClaims = context.Principal?.Claims.ToArray();

        if (principalClaims?.Any() != true)
            return false;
        
        string? nameIdentifier = GetGuidFromClaims(principalClaims);

        if (string.IsNullOrWhiteSpace(nameIdentifier))
            return false;

        DatabaseContext dbContext = context.HttpContext.RequestServices.GetRequiredService<DatabaseContext>();

        Guid nameIdentifierGuid = new(nameIdentifier);
        
        bool isExist = await dbContext.UserAccounts
            .Where(ua => ua.Id == nameIdentifierGuid)
            .Where(ua => ua.BannedUntil == null || ua.BannedUntil < DateTime.Now)
            .AnyAsync();

        if (!isExist)
            return false;

        return true;
    }

    /// <inheritdoc />
    public string? GetGuidFromClaims(IEnumerable<Claim> principalClaims)
    {
        return principalClaims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
    }
}