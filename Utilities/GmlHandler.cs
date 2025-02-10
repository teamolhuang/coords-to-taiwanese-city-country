using System.Xml;
using NetTopologySuite.Geometries;
using NetTopologySuite.Index.Strtree;
using NetTopologySuite.IO.GML2;

namespace coords_to_taiwanese_city_country.Utilities;

/// <summary>
/// 針對 鄉鎮市區界線(TWD97經緯度) 的 gml 進行處理的包裝。
/// </summary>
public static class GmlHandler
{
    /// <summary>
    /// 傳入經度、緯度，回傳縣市與鄉鎮市區。
    /// </summary>
    /// <param name="longitude">經度</param>
    /// <param name="latitude">緯度</param>
    /// <returns>
    /// 縣市與鄉鎮市區的中文字串的 tuple
    /// </returns>
    public static async Task<(string city, string country)?> GetAddressAsync(decimal longitude, decimal latitude)
    {
        string closest = await QueryGeometryAsync(longitude, latitude);

        // 台灣的縣市名稱固定為三個字元
        return (new string(closest.Take(3).ToArray()), new string(closest.Skip(3).ToArray()));
    }
    
    #region privates
    
    private static STRtree<Geometry>? _spatialIndex;

    private const string NameNode = "名稱";
    private const string GmlMultiPolygonNode = "gml:MultiPolygon";
    
    /// <summary>
    /// 初始化
    /// </summary>
    private static async Task InitializeIndexAsync()
    {
        // GML 格式基於 XML，而政府開放資料平台提供的資料是一個包含許多 GML 元素的 XML
        // 所以我們用讀取 XML -> 撈出 GML 的方式進行 
        XmlReader xmlReader = XmlReader.Create("StaticFiles/twd97.gml", new XmlReaderSettings
        {
            Async = true
        });
        
        // 我們的最終目標是把所有行政區建成一棵 R-Tree，常用來存取空間資料的一種資料結構
        _spatialIndex = new STRtree<Geometry>();

        GMLReader gmlReader = new();
        string nowName = "";
        
        // 初步分析，一個 <gml:MultiPolygon> 代表一個行政區，同時它也就是一個完整的 GML（並不是整個檔案等於一個 GML 的意思）
        // 所以用 xmlReader 跑過整個 XML，遇到這種元素才用 gmlReader
        while (await xmlReader.ReadAsync())
        {
            // 我們關注兩種元素
            // <名稱>xx市xx區</名稱>：表示接下來這個 GML 元素代表哪個行政區
            // <gml:MultiPolygon>...</gml:MultiPolygon>：透過 NetTopologySuite 轉成 Geometry 並存進 R-Tree

            if (xmlReader.NodeType != XmlNodeType.Element)
                continue;
            
            if (xmlReader.Name.Equals(NameNode))
            {
                nowName = await xmlReader.ReadElementContentAsStringAsync();
            } 
            
            if (xmlReader.Name.Equals(GmlMultiPolygonNode))
            {
                Geometry? geometry = gmlReader.Read(xmlReader);

                if (geometry == null) continue;
                
                SetLocationName(geometry, nowName);
                _spatialIndex.Insert(geometry.EnvelopeInternal, geometry);
            }
        }
        
    }
    
    /// <summary>
    /// 輸入經緯度，基於 TWD97 查詢縣市、行政區。
    /// </summary>
    /// <returns>縣市與區的字串，例如「桃園市桃園區」。</returns>
    private static async Task<string> QueryGeometryAsync(decimal longitude, decimal latitude)
    {
        // 如果這次生命週期還沒有建立過縣市/行政區的 R-Tree，先初始化
        if (_spatialIndex is null)
            await InitializeIndexAsync();
        
        // 把指定的經緯度轉成矩形以便搜尋
        Coordinate coordinate = new(Convert.ToDouble(longitude), Convert.ToDouble(latitude));
        Envelope queryEnvelope = new(coordinate);
        
        // 查矩形範圍有相交的所有行政區
        IList<Geometry>? queried = _spatialIndex!.Query(queryEnvelope);
        
        // 相交的資料回來會是「Geometry」這個類型，
        // 因此把目前搜尋的點也轉成 Geometry 以便求出最近的行政區
        GeometryFactory factory = GeometryFactory.FloatingSingle;
        Geometry searchingLocation = factory.ToGeometry(queryEnvelope);
        
        Geometry? nearest = queried?
            .OrderBy(g => g.Distance(searchingLocation))
            .FirstOrDefault();

        return GetLocationName(nearest) ?? "";
    }

    /// <summary>
    /// 從 Geometry 嘗試取出縣市行政區的中文字串
    /// </summary>
    private static string? GetLocationName(Geometry? geometry)
    {
        return (string?)geometry?.UserData;
    }

    /// <summary>
    /// 把縣市行政區的中文字串存入 Geometry 的自訂欄位
    /// </summary>
    private static void SetLocationName(Geometry geometry, string name)
    {
        // Geometry 提供 UserData 這個自訂欄位
        // 為了後續使用方便，我們統一把縣市與行政區的中文字串放到 UserData
        geometry.UserData = name;
    }
    
    #endregion
}