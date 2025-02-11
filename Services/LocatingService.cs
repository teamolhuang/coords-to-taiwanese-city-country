using System.Diagnostics.CodeAnalysis;
using coords_to_taiwanese_city_country.Models;
using coords_to_taiwanese_city_country.Services.Abstracts;
using coords_to_taiwanese_city_country.Utilities;
using coords_to_taiwanese_city_country.Utilities.Abstracts;

namespace coords_to_taiwanese_city_country.Services;

/// <inheritdoc />
public class LocatingService(ILogger<ILocatingService> logger,
    IGmlHandler gmlHandler) : ILocatingService
{
    /// <inheritdoc />
    public async Task<GetLocationResponse> GetLocationInTaiwanMainLandAsync(GetLocationRequest request)
    {
        try
        {
            // 1. 呼叫 GmlHandler，取得 XX市 XX區 的字串。
            (string city, string country)? result =
                await gmlHandler.GetAddressAsync(request.Longitude!.Value, request.Latitude!.Value);

            // 2. 如果沒有結果，拋出。
            if (result is null 
                || (string.IsNullOrWhiteSpace(result.Value.city) && string.IsNullOrWhiteSpace(result.Value.country)))
                throw new NullReferenceException($"查無資料，請確認經緯度（{request.Longitude}, {request.Latitude}）是否正確");

            // 3. 把結果格式化回傳。
            return new GetLocationResponse
            {
                City = result.Value.city,
                Country = result.Value.country
            };
        }
        catch (Exception e)
        {
            logger.LogError(e, "查詢失敗");
            throw;
        }
    }

    private static CityCountryListResponse? _listCache;
    /// <inheritdoc />
    public async Task<CityCountryListResponse> GetListAsync()
    {
        if (_listCache is null)
            await CreateListCacheAsync();

        return _listCache;
    }

    [MemberNotNull(nameof(_listCache))]
    private async Task CreateListCacheAsync()
    {
        IEnumerable<string> allCityAndCountry = await gmlHandler.GetListAsync();
        
        // 台灣的縣市名固定只有三個字，所以每個字串取前三個字當縣市的 key，後面就是鄉鎮市區了。

        ILookup<string, string> lookup = allCityAndCountry
            .ToLookup(s => new string(s.Take(3).ToArray()), s => new string(s.Skip(3).ToArray()));

        _listCache = new CityCountryListResponse
        {
            Cities = lookup
                .Select(g => new CityCountryListResponseCity
                {
                    Name = g.Key,
                    Countries = g
                })
                .OrderBy(c => c.Name)
        };
    }
}