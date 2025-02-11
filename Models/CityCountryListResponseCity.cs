namespace coords_to_taiwanese_city_country.Models;

/// <summary>
/// 取得縣市與鄉鎮市區列表時的回傳物件中，表示單一縣市的子物件。
/// </summary>
public class CityCountryListResponseCity
{
    /// <summary>
    /// 縣市名稱。
    /// </summary>
    /// <example>新北市</example>
    public string Name { get; set; } = null!;

    /// <summary>
    /// 此縣市鄉鎮市區的總數。
    /// </summary>
    public int CountryCount => Countries.Count();
    
    /// <summary>
    /// 鄉鎮市區的集合。
    /// </summary>
    public IEnumerable<string> Countries { get; set; } = [];
}