namespace coords_to_taiwanese_city_country.Models;

/// <summary>
/// 取得縣市與鄉鎮市區列表時的回傳物件。
/// </summary>
public class CityCountryListResponse
{
    /// <summary>
    /// 縣市的總數。
    /// </summary>
    public int CityCount => Cities.Count();

    /// <summary>
    /// 鄉鎮市區的總數。
    /// </summary>
    public int CountryCount => Cities.SelectMany(c => c.Countries).Count();
    
    /// <summary>
    /// 縣市的集合。
    /// </summary>
    public IEnumerable<CityCountryListResponseCity> Cities { get; set; } = [];
}