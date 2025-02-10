using System.ComponentModel.DataAnnotations;

namespace coords_to_taiwanese_city_country.Models;

/// <summary>
/// 註冊時的傳入物件
/// </summary>
public class RegisterRequest
{
    /// <summary>
    /// 帳號，6~20 個字
    /// </summary>
    /// <example>userExample</example>
    [MinLength(6, ErrorMessage = "帳號最小應有 6 個字！")]
    [MaxLength(20, ErrorMessage = "帳號最大限 20 個字！")]
    [RegularExpression("^[a-zA-Z0-9]+$", ErrorMessage = "帳號只允許輸入半形英文字母，或半形數字！")]
    [Required]
    public string Account { get; set; } = null!;

    /// <summary>
    /// 密碼，至少 8 個字
    /// </summary>
    /// <example>12345678</example>
    [MinLength(8, ErrorMessage = "密碼最小應有 8 個字！")]
    [Required]
    public string Password { get; set; } = null!;
}