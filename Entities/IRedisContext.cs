using StackExchange.Redis;

namespace coords_to_taiwanese_city_country.Entities;

/// <summary>
/// Redis 資料庫的抽象化。
/// </summary>
public interface IRedisContext
{
    /// <summary>
    /// 對 Redis 資料庫進行操作的入口。
    /// </summary>
    IDatabase Database { get; }
}