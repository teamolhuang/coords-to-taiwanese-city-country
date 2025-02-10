using System.Security.Claims;
using coords_to_taiwanese_city_country.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;

namespace coords_to_taiwanese_city_country.Utilities;

/// <summary>
/// 處理 JWT 中代表使用者 ID 的 guid
/// </summary>
public static class JwtGuidHandler
{
    /// <summary>
    /// 驗證 claim 中的 GUID 尚未在本系統被刪除
    /// </summary>
    public static async Task<bool> ValidateGuidNotDeletedAsync(TokenValidatedContext context)
    {
        Claim[]? principalClaims = context.Principal?.Claims.ToArray();

        if (principalClaims?.Any() != true)
            return false;
        
        string? nameIdentifier = principalClaims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrWhiteSpace(nameIdentifier))
            return false;

        DatabaseContext dbContext = context.HttpContext.RequestServices.GetRequiredService<DatabaseContext>();

        Guid nameIdentifierGuid = new(nameIdentifier);
        
        bool isExist = await dbContext.UserAccounts
            .Where(ua => ua.Id == nameIdentifierGuid)
            .AnyAsync();

        if (!isExist)
            return false;

        return true;
    }
}