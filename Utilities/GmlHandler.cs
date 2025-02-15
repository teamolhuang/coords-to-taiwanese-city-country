using System.Xml;
using coords_to_taiwanese_city_country.Entities;
using coords_to_taiwanese_city_country.Utilities.Abstracts;
using StackExchange.Redis;

namespace coords_to_taiwanese_city_country.Utilities;

/// <summary>
/// 針對 鄉鎮市區界線(TWD97經緯度) 的 gml 進行處理的包裝。
/// </summary>
public class GmlHandler(IRedisContext redisContext) : IGmlHandler
{
    /// <inheritdoc />
    public async Task<(string city, string country)?> GetAddressAsync(decimal longitude, decimal latitude)
    {
        string closest = await QueryForNearestCityCountry(longitude, latitude);

        if (string.IsNullOrWhiteSpace(closest))
            return null;
        
        // 台灣的縣市名稱固定為三個字元
        return (new string(closest.Take(3).ToArray()), new string(closest.Skip(3).ToArray()));
    }

    /// <inheritdoc />
    public async Task<IEnumerable<string>> GetListAsync()
    {
        await EnsureRedisGeoSetInitializedAsync();

        RedisValue[] values = await redisContext.Database.SetMembersAsync(RedisContextKeys.CoordinateNames);

        return values.Select(v => v.ToString());
    }

    #region privates

    private const string NameNode = "名稱";
    private const string GmlCoordinatesNode = "gml:coordinates";
    
    /// <summary>
    /// 初始化
    /// </summary>
    private async Task EnsureRedisGeoSetInitializedAsync()
    {
        // 如果已經建立過的
        if (redisContext.Database.KeyExists(RedisContextKeys.Coordinates))
            return;
        
        // GML 格式基於 XML，而政府開放資料平台提供的資料是一個包含許多 GML 元素的 XML
        // 我們用讀取 XML -> 撈出 GML 的方式進行 
        XmlReader xmlReader = XmlReader.Create("StaticFiles/twd97.gml", new XmlReaderSettings
        {
            Async = true
        });

        // 我們的目標是把 GML 中所有行政區與它對應的 bounding 經緯度轉成 Geo 存在 Redis
        // 同時，也把系統中所有名稱存起來，方便確認目前系統支援哪些縣市／鄉鎮市區
        
        string nowName = "";
        
        while (await xmlReader.ReadAsync())
        {
            // 我們關注兩種元素
            // <名稱>xx市xx區</名稱>：表示接下來這個 GML 元素代表哪個行政區
            // <gml:coordinates>...</gml:coordinates>：這個行政區有哪些經緯度

            if (xmlReader.NodeType != XmlNodeType.Element)
                continue;
            
            if (xmlReader.Name.Equals(NameNode))
            {
                nowName = await xmlReader.ReadElementContentAsStringAsync();
                await redisContext.Database.SetAddAsync(RedisContextKeys.CoordinateNames, nowName);
            } 
            
            if (xmlReader.Name.Equals(GmlCoordinatesNode))
            {
                string fullBlock = await xmlReader.ReadElementContentAsStringAsync();
                
                // 依空白分隔，就是 經,緯 的字串的集合
                string[] allCoordinates = fullBlock.Split(' ');
                string name = nowName;
                
                IEnumerable<GeoEntry> entries = allCoordinates.Select(coord =>
                {
                    double[] coords = coord
                        .Split(',')
                        .Select(Convert.ToDouble)
                        .ToArray();
                    
                    return new GeoEntry(coords.First(), coords.Last(), name);
                });

                await redisContext.Database.GeoAddAsync(RedisContextKeys.Coordinates, entries.ToArray());
            }
        }
    }
    
    /// <summary>
    /// 輸入經緯度，基於 TWD97 查詢縣市、行政區。
    /// </summary>
    /// <returns>縣市與區的字串，例如「桃園市桃園區」。</returns>
    private async Task<string> QueryForNearestCityCountry(decimal longitude, decimal latitude)
    {
        // 參照維基百科，台灣最大面積的行政區是 1641.85 平方公里
        // 我們半徑查 1642 / 2 = 821
        GeoRadiusResult[] closest = await redisContext.Database.GeoSearchAsync(RedisContextKeys.Coordinates,
            (double)longitude,
            (double)latitude,
            new GeoSearchCircle(821, GeoUnit.Kilometers),
            1);

        return closest.FirstOrDefault()
            .Member
            .ToString();
    }
    
    #endregion
}