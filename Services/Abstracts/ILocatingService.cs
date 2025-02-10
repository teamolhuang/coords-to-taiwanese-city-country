using coords_to_taiwanese_city_country.Models;

namespace coords_to_taiwanese_city_country.Services.Abstracts;

/// <summary>
/// 地理位置相關的商業邏輯
/// </summary>
public interface ILocatingService
{
    /// <summary>
    /// 傳入經、緯度，查詢最接近的台灣本島縣市/鄉鎮市區。
    /// </summary>
    /// <returns>
    /// <see cref="GetLocationResponse"/>
    /// </returns>
    public Task<GetLocationResponse> GetLocationInTaiwanMainLandAsync(GetLocationRequest request);
}