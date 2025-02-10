namespace coords_to_taiwanese_city_country.Models;

/// <summary>
/// 本 API 的通用回傳格式
/// </summary>
public class BaseResponse
{
    /// <summary>
    /// 此次商業邏輯執行是否成功。為 true 時，Result 保證有符合規格的值。
    /// </summary>
    public bool IsSuccessful { get; set; }
    
    /// <summary>
    /// 執行結果的相關訊息。執行錯誤時，會將錯誤訊息放在這裡。
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// 取得實例。
    /// </summary>
    /// <param name="errorMessage">（可選）執行失敗時，傳入錯誤訊息</param>
    public BaseResponse(string? errorMessage = null)
    {
        IsSuccessful = string.IsNullOrWhiteSpace(errorMessage);
        Message = errorMessage;
    }
}

/// <summary>
/// 本 API 的通用回傳格式，包含子物件
/// </summary>
public class BaseResponse<TResult> : BaseResponse
{
    /// <summary>
    /// 執行結果。
    /// </summary>
    public TResult? Result { get; set; }
    
    /// <summary>
    /// 取得實例
    /// </summary>
    /// <param name="result">執行成功的回傳物件</param>
    public BaseResponse(TResult result)
    {
        IsSuccessful = true;
        Result = result;
    }
    
    /// <summary>
    /// 取得實例
    /// </summary>
    /// <param name="ex">執行失敗時發生的拋錯</param>
    public BaseResponse(Exception ex)
    {
        IsSuccessful = false;
        Message = ex.Message;
    }
}