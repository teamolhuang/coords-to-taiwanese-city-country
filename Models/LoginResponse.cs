namespace coords_to_taiwanese_city_country.Models;

/// <summary>
/// 登入後的回傳物件
/// </summary>
public class LoginResponse
{
    /// <summary>
    /// JWT 權杖
    /// </summary>
    public string AccessToken { get; set; } = null!;
    
    /// <summary>
    /// JWT 有效期限
    /// </summary>
    public DateTime ExpiresAt { get; set; }
}