namespace coords_to_taiwanese_city_country.Utilities;

/// <summary>
/// 用於表示類似 429 的情況時的 Exception
/// </summary>
public class TooManyRequestsException(string message) : Exception(message)
{
    
}