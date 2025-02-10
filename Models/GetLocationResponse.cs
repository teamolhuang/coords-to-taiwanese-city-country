namespace coords_to_taiwanese_city_country.Models;

/// <summary>
/// 查詢縣市/鄉鎮市區時的回傳結果。
/// </summary>
public class GetLocationResponse
{
    /// <summary>
    /// 縣市，包含「縣」或「市」等後綴。
    /// </summary>
    /// <example>基隆市</example>
    public string City { get; set; } = null!;

    /// <summary>
    /// 鄉鎮市區，包含「區」等後綴。
    /// </summary>
    /// <example>中山區</example>
    public string Country { get; set; } = null!;
}