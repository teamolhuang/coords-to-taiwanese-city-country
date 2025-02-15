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
    public Task<bool> ValidateGuidNotDeletedAsync(TokenValidatedContext context)
    {
        Claim[]? principalClaims = context.Principal?.Claims.ToArray();

        if (principalClaims?.Any() != true)
            return Task.FromResult(false);
        
        string? nameIdentifier = GetGuidFromClaims(principalClaims);

        if (string.IsNullOrWhiteSpace(nameIdentifier))
            return Task.FromResult(false);

        // 檢查 Redis 刪除帳號用的暫存表中，是否存在這個 GUID。如果存在，就當成驗證不通過。
        IRedisContext redisContext = context.HttpContext.RequestServices.GetRequiredService<IRedisContext>();
        
        bool isAllowed = !redisContext.Database.SetContains(RedisContextKeys.DeletedAccount, nameIdentifier);  
        
        return Task.FromResult(isAllowed);
    }

    /// <inheritdoc />
    public string? GetGuidFromClaims(IEnumerable<Claim> principalClaims)
    {
        return principalClaims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
    }
}