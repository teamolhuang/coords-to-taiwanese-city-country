using coords_to_taiwanese_city_country.Models;
using coords_to_taiwanese_city_country.Services.Abstracts;
using coords_to_taiwanese_city_country.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace coords_to_taiwanese_city_country.Controllers;

/// <summary>
/// 地理位置相關的控制器
/// </summary>
[ApiController]
[Route("api/locating")]
public class LocatingController(
        ILocatingService locatingService
    ) : ControllerBase
{
    /// <summary>
    /// 傳入經緯度，轉換成台灣本島與該座標最鄰近的縣市、鄉鎮市區。
    /// </summary>
    [HttpGet("coords-to-taiwanese-city-country")]
    [AllowAnonymous]
    [ProducesResponseType<BaseResponse<GetLocationResponse>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTaiwanCityCountry([FromQuery] GetLocationRequest request)
    {
        BaseResponse<GetLocationResponse> result = await locatingService.GetLocationInTaiwanMainLandAsync(request)
            .ToBaseResponse();

        return Ok(result);
    }
}