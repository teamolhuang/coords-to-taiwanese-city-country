using coords_to_taiwanese_city_country.Models;
using coords_to_taiwanese_city_country.Services;
using coords_to_taiwanese_city_country.Services.Abstracts;
using coords_to_taiwanese_city_country.Utilities;
using coords_to_taiwanese_city_country.Utilities.Abstracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace coords_to_taiwanese_city_country.Controllers;

/// <summary>
/// 地理位置相關的控制器
/// </summary>
[ApiController]
[Authorize]
[Route("api/locating")]
public class LocatingController(
        ILocatingService locatingService,
        ICooldownService cooldownService,
        IAuthService authService,
        IJwtGuidHandler jwtGuidHandler
    ) : ControllerBase
{
    /// <summary>
    /// 傳入經緯度，轉換成台灣本島與該座標最鄰近的縣市、鄉鎮市區。
    /// </summary>
    [HttpGet]
    [ProducesResponseType<BaseResponse<GetLocationResponse>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTaiwanCityCountry([FromQuery] GetLocationRequest request)
    {
        Func<Task<GetLocationResponse>> execution = async () =>
        {
            string userIdentifier = jwtGuidHandler.GetGuidFromClaims(HttpContext.User.Claims)!;

            try
            {
                await cooldownService.CheckCooldownAsync(TimeSpan.FromSeconds(1),
                    100,
                    userIdentifier);
            }
            catch (TooManyRequestsException)
            {
                // 如果冷卻檢查失敗，而且錯誤確實是超出限制了，跑去 ban 一小時
                await authService.BanAsync(userIdentifier, TimeSpan.FromHours(1));
                throw;
            }

            return await locatingService.GetLocationInTaiwanMainLandAsync(request);
        };

        BaseResponse<GetLocationResponse> result = await execution.Invoke().ToBaseResponse();

        return Ok(result);
    }

    /// <summary>
    /// 查詢目前本系統中的所有縣市與鄉鎮市區。
    /// </summary>
    [HttpGet("list")]
    [ProducesResponseType<CityCountryListResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCityCountryList()
    {
        BaseResponse<CityCountryListResponse> result = await locatingService.GetListAsync()
            .ToBaseResponse();

        return Ok(result);
    }
}