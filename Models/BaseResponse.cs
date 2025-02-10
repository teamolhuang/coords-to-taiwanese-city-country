namespace coords_to_taiwanese_city_country.Models;

/// <summary>
/// 本 API 的通用回傳格式
/// </summary>
public class BaseResponse<TResult>
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
    /// 執行結果。
    /// </summary>
    public TResult? Result { get; set; }

    public BaseResponse(TResult result)
    {
        IsSuccessful = true;
        Result = result;
    }

    public BaseResponse(Exception ex)
    {
        IsSuccessful = false;
        Message = ex.Message;
    }
}