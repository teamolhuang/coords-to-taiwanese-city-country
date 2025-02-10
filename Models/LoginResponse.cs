namespace coords_to_taiwanese_city_country.Models;

/// <summary>
/// 登入後的回傳物件
/// </summary>
public class LoginResponse
{
    private string _accessToken = null!;

    /// <summary>
    /// JWT 權杖
    /// </summary>
    public string AccessToken
    {
        get => _accessToken;
        set =>
            _accessToken = value.StartsWith("Bearer ", StringComparison.InvariantCultureIgnoreCase)
                ? value
                : $"Bearer {value}";
    }

    /// <summary>
    /// JWT 有效期限
    /// </summary>
    public DateTime ExpiresAt { get; set; }
}