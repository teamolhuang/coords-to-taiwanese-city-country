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
    [Required]
    public decimal? Longitude { get; set; }
    
    /// <summary>
    /// 緯度
    /// </summary>
    [Required]
    public decimal? Latitude { get; set; }
}