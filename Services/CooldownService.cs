using System.Collections.Concurrent;
using System.Net;
using System.Runtime.CompilerServices;
using coords_to_taiwanese_city_country.Services.Abstracts;
using coords_to_taiwanese_city_country.Utilities;

namespace coords_to_taiwanese_city_country.Services;

/// <inheritdoc />
public class CooldownService : ICooldownService
{
    private static readonly Dictionary<(string userIdentifier, string methodIdentifier), ConcurrentQueue<DateTime>> CooldownCache = new();
    
    /// <inheritdoc />
    public Task CheckCooldownAsync(TimeSpan duration, int executionLimit, string userIdentifier, 
        [CallerFilePath] string callerPath = "",
        [CallerMemberName] string memberName = "")
    {
        // 利用 user 識別子跟方法的識別子，建立一個在記憶體中的 CooldownCache
        // 每個複合識別子會有一個時間從最舊到最新的 Queue
        // 利用這個 Queue 來判定時間內是否已超過指定次數

        // 1. 找出識別子對應的 queue，沒有就建一個
        (string, string) key = (userIdentifier, $"{callerPath}-{memberName}");

        if (!CooldownCache.ContainsKey(key))
            CooldownCache[key] = new ConcurrentQueue<DateTime>();
        
        ConcurrentQueue<DateTime> histories = CooldownCache[key];
        
        // 2. 把 Queue 中已經過舊的歷史記錄清光
        DateTime earliest = DateTime.Now.Add(-duration);
        while (histories.TryPeek(out DateTime dt))
        {
            if (earliest <= dt)
                break;
            
            histories.TryDequeue(out _);
        }
        
        // 3. 檢查 Queue 長度，如果大於等於 executionLimit 就代表已經達上限了，冷靜一下
        if (histories.Count >= executionLimit)
            throw new TooManyRequestsException($"近期執行次數已超過 {executionLimit} 次");
        
        // 4. 否則，在 Queue 塞一筆新的紀錄，然後結束
        histories.Enqueue(DateTime.Now);
        return Task.CompletedTask;
    }
}