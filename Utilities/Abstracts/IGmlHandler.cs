using coords_to_taiwanese_city_country.Models;

namespace coords_to_taiwanese_city_country.Utilities.Abstracts;

public interface IGmlHandler
{
    /// <summary>
    /// 傳入經度、緯度，回傳縣市與鄉鎮市區。
    /// </summary>
    /// <param name="longitude">經度</param>
    /// <param name="latitude">緯度</param>
    /// <returns>
    /// 縣市與鄉鎮市區的中文字串的 tuple
    /// </returns>
    Task<(string city, string country)?> GetAddressAsync(decimal longitude, decimal latitude);

    /// <summary>
    /// 取得 GML 中包含的所有縣市與鄉鎮市區。
    /// </summary>
    Task<IEnumerable<string>> GetListAsync();
}