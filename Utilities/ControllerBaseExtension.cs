using coords_to_taiwanese_city_country.Models;

namespace coords_to_taiwanese_city_country.Utilities;

/// <summary>
/// Controller 的擴充方法。
/// </summary>
public static class ControllerBaseExtension
{
    /// <summary>
    /// 向後呼叫執行商業邏輯，並且自動包裝結果成通用回傳格式的 wrapper。
    /// </summary>
    public static async Task<BaseResponse<T>> ToBaseResponse<T>(this Task<T> task)
    {
        try
        {
            T result = await task;
            return new BaseResponse<T>(result);
        }
        catch (Exception e)
        {
            return new BaseResponse<T>(e);
        }
    }
    
    /// <summary>
    /// 向後呼叫執行商業邏輯，並且回傳通用回傳格式。
    /// </summary>
    public static async Task<BaseResponse> ToBaseResponse(this Task task)
    {
        try
        {
            await task;
            return new BaseResponse();
        }
        catch (Exception e)
        {
            return new BaseResponse(e.Message);
        }
    }
}