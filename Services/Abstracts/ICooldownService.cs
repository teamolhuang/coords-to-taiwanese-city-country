using System.Runtime.CompilerServices;

namespace coords_to_taiwanese_city_country.Services.Abstracts;

/// <summary>
/// 處理 API throttling 的商業邏輯
/// </summary>
public interface ICooldownService
{
    /// <summary>
    /// 檢查此次要求是否已超出 API 次數限制。如果超出限制，會拋錯。
    /// </summary>
    Task CheckCooldownAsync(TimeSpan duration, 
        int executionLimit, 
        string userIdentifier, 
        [CallerFilePath] string callerPath = "", 
        [CallerMemberName] string memberName = "");
}