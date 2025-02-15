namespace coords_to_taiwanese_city_country.Utilities.Abstracts;

/// <summary>
/// JWT Token 發行相關的工具
/// </summary>
public interface IJwtTokenHelper
{
    /// <summary>
    /// 發行 JWT
    /// </summary>
    string GenerateToken(Guid userId, string privateKey);

    /// <summary>
    /// 取得本系統的 JWT 有效時間長度
    /// </summary>
    TimeSpan GetExpirationSpan();
}