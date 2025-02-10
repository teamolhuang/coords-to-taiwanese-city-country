using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace coords_to_taiwanese_city_country.Entities;

/// <summary>
/// 使用者帳號
/// </summary>
[Table("UserAccount")]
[Description("使用者帳號")]
public class UserAccount
{
    /// <summary>
    /// 流水號
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    /// <summary>
    /// 使用者帳號，字元不限，6~20 個字
    /// </summary>
    [MinLength(6)]
    [MaxLength(20)]
    [Required]
    public string Account { get; set; } = null!;

    /// <summary>
    /// 雜湊過的密碼
    /// </summary>
    public string HashedPassword { get; set; } = null!;
    
    /// <summary>
    /// 禁用冷卻期限
    /// </summary>
    public DateTimeOffset? BannedUntil { get; set; }
}