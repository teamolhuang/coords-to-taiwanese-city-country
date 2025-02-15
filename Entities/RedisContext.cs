using StackExchange.Redis;

namespace coords_to_taiwanese_city_country.Entities;

/// <inheritdoc />
public class RedisContext : IRedisContext
{
    /// <summary>
    /// 取得實例。
    /// </summary>
    public RedisContext(IConfiguration configuration)
    {
        // 從設定檔中取得 Redis 的連線字串
        string connectionString = configuration
            .GetSection("Redis")
            .GetValue<string?>("ConnectionString") ?? throw new NullReferenceException("RedisConnectionString not set!");
        
        // 建立連線 - Redis 的連線管理形式不是多執行緒，所以避免重複開連線消耗資源，在初始化時就建立對 db 的連線。
        Redis = ConnectionMultiplexer.Connect(connectionString);
    }
    
    private ConnectionMultiplexer Redis { get; init; }

    /// <inheritdoc />
    public IDatabase Database => Redis.GetDatabase();
}
