using System.Security.Claims;
using coords_to_taiwanese_city_country.Models;

namespace coords_to_taiwanese_city_country.Services.Abstracts;

/// <summary>
/// 處理使用者身分相關的商業邏輯
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// 註冊
    /// </summary>
    Task<RegisterResponse> RegisterAsync(RegisterRequest request);

    /// <summary>
    /// 登入
    /// </summary>
    Task<LoginResponse> LoginAsync(LoginRequest request);

    /// <summary>
    /// 刪除帳號
    /// </summary>
    Task DeleteAsync(ClaimsPrincipal claimsPrincipal);
}