namespace coords_to_taiwanese_city_country.Entities;

/// <summary>
/// 用於取得 Redis 各種 keys 的統一來源。
/// </summary>
public static class RedisContextKeys
{
    /// <summary>
    /// 刪除帳號後，在 Redis 中暫存 GUID 用的表。
    /// </summary>
    public const string DeletedAccount = "DeletedAccount";
    
    /// <summary>
    /// 所有包含在 GML 中的縣市與鄉鎮市區的經緯度座標
    /// </summary>
    public const string Coordinates = "coords";
    
    /// <summary>
    /// 所有包含在 GML 中的縣市與鄉鎮市區的名稱
    /// </summary>
    public const string CoordinateNames = "coords-names";
}