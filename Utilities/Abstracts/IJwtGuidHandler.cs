using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace coords_to_taiwanese_city_country.Utilities.Abstracts;

/// <summary>
/// 處理 JWT 中代表使用者 ID 的 guid
/// </summary>
public interface IJwtGuidHandler
{
    /// <summary>
    /// 從 claims 中取得當前使用者的 GUID
    /// </summary>
    string? GetGuidFromClaims(IEnumerable<Claim> principalClaims);

    /// <summary>
    /// 驗證 claim 中的 GUID 尚未在本系統被刪除
    /// </summary>
    Task<bool> ValidateGuidNotDeletedAsync(TokenValidatedContext context);
}