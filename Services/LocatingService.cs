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
}