namespace coords_to_taiwanese_city_country.Models;

/// <summary>
/// 註冊時的回傳物件
/// </summary>
public class RegisterResponse
{
    /// <summary>
    /// 註冊成功的帳號
    /// </summary>
    public string Account { get; set; } = null!;
}