using System.Security.Claims;
using coords_to_taiwanese_city_country.Models;
using coords_to_taiwanese_city_country.Services;
using coords_to_taiwanese_city_country.Services.Abstracts;
using coords_to_taiwanese_city_country.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace coords_to_taiwanese_city_country.Controllers;

/// <summary>
/// 使用者登入、驗證等相關的控制器
/// </summary>
[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService authService) : ControllerBase
{
    /// <summary>
    /// 註冊
    /// </summary>
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType<BaseResponse<RegisterResponse>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> RegisterAsync([FromBody] RegisterRequest request)
    {
        BaseResponse<RegisterResponse> result = await authService.RegisterAsync(request)
            .ToBaseResponse();

        return Ok(result);
    }
    
    /// <summary>
    /// 登入
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType<BaseResponse<LoginResponse>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request)
    {
        BaseResponse<LoginResponse> result = await authService.LoginAsync(request)
            .ToBaseResponse();

        return Ok(result);

    }
    
    /// <summary>
    /// 刪除帳號
    /// </summary>
    [HttpDelete("delete-account")]
    [Authorize]
    [ProducesResponseType<BaseResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteAccountAsync()
    {
        BaseResponse result = await authService.DeleteAsync(HttpContext.User)
            .ToBaseResponse();

        return Ok(result);

    }
}