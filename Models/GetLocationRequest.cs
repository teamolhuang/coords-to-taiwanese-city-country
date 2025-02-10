using System.ComponentModel.DataAnnotations;

namespace coords_to_taiwanese_city_country.Models;

/// <summary>
/// 查詢縣市/鄉鎮市區時的傳入要求。
/// </summary>
public class GetLocationRequest
{
    /// <summary>
    /// 經度
    /// </summary>
    /// <example>121.7238251</example>
    [Required]
    public decimal? Longitude { get; set; }
    
    /// <summary>
    /// 緯度
    /// </summary>
    /// <example>25.1524525</example>
    [Required]
    public decimal? Latitude { get; set; }
}