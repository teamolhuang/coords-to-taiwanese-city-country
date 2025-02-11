using coords_to_taiwanese_city_country.Models;
using coords_to_taiwanese_city_country.Services;
using coords_to_taiwanese_city_country.Utilities.Abstracts;
using Moq;
using Moq.AutoMock;

namespace tests.Services.LocatingServiceTests;

[TestFixture]
[Description("針對 LocatingService 中轉換經緯度方法的各種測試。")]
public class LocatingServiceGetLocationTests
{
    [Test]
    [TestCase(121.6, 25.1, "基隆市", "中山區")]
    [TestCase(118.1805, 23.4827, "台北市", "大安區")]
    [TestCase(121.2189, 24.9639, "桃園市", "中壢區")]
    [Description("驗證 GetLocationInTaiwanMainLandAsync 方法應該要呼叫 GmlHandler，把經緯度轉換成對應的縣市與鄉鎮市區。")]
    public async Task GetLocationInTaiwanMainLandAsync_ShouldCallGmlHandler_AndReturnLocation(
        decimal longitude,
        decimal latitude,
        string expectedCity,
        string expectedCountry
        )
    {
        // Arrange
        AutoMocker autoMocker = new();

        Mock<IGmlHandler> mockGmlHandler = autoMocker.GetMock<IGmlHandler>();
        mockGmlHandler
            .Setup(handler => handler.GetAddressAsync(longitude, latitude))
            .ReturnsAsync((expectedCity, expectedCountry))
            .Verifiable(Times.Once);
        
        GetLocationRequest request = new()
        {
            Longitude = longitude,
            Latitude = latitude
        };
        
        LocatingService locatingService = autoMocker.Get<LocatingService>();
        
        // Act
        GetLocationResponse result = await locatingService.GetLocationInTaiwanMainLandAsync(request);

        // Assert
        
        Assert.That(result.City, Is.EqualTo(expectedCity));
        Assert.That(result.Country, Is.EqualTo(expectedCountry));
        mockGmlHandler.VerifyAll();
    }
    
    [Test]
    [Description("驗證 GetLocationInTaiwanMainLandAsync 方法呼叫 GmlHandler 後，如果結果為空，應該拋出 NullReferenceException。")]
    public Task GetLocationInTaiwanMainLandAsync_ShouldCallGmlHandler_AndThrowNullReferenceExceptionOnEmptyResult()
    {
        // Arrange
        AutoMocker autoMocker = new();

        Mock<IGmlHandler> mockGmlHandler = autoMocker.GetMock<IGmlHandler>();
        mockGmlHandler
            .Setup(handler => handler.GetAddressAsync(It.IsAny<decimal>(), It.IsAny<decimal>()))
            .ReturnsAsync((string.Empty, string.Empty))
            .Verifiable(Times.Once);
        
        Random random = new();
        
        GetLocationRequest request = new()
        {
            Longitude = new decimal(random.NextDouble()),
            Latitude = new decimal(random.NextDouble())
        };
        
        LocatingService locatingService = autoMocker.Get<LocatingService>();
        
        // Act & Assert
        Assert.ThrowsAsync<NullReferenceException>(async () => await locatingService.GetLocationInTaiwanMainLandAsync(request));

        // Assert
        mockGmlHandler.VerifyAll();
        
        return Task.CompletedTask;
    }
    
    [Test]
    [Description("驗證 GetLocationInTaiwanMainLandAsync 方法呼叫 GmlHandler 後，如果結果為 null，應該拋出 NullReferenceException。")]
    public Task GetLocationInTaiwanMainLandAsync_ShouldCallGmlHandler_AndThrowNullReferenceExceptionOnNullResult()
    {
        // Arrange
        AutoMocker autoMocker = new();

        Mock<IGmlHandler> mockGmlHandler = autoMocker.GetMock<IGmlHandler>();
        mockGmlHandler
            .Setup(handler => handler.GetAddressAsync(It.IsAny<decimal>(), It.IsAny<decimal>()))
            .ReturnsAsync(((string, string)?)null)
            .Verifiable(Times.Once);
        
        Random random = new();
        
        GetLocationRequest request = new()
        {
            Longitude = new decimal(random.NextDouble()),
            Latitude = new decimal(random.NextDouble())
        };
        
        LocatingService locatingService = autoMocker.Get<LocatingService>();
        
        // Act & Assert
        Assert.ThrowsAsync<NullReferenceException>(async () => await locatingService.GetLocationInTaiwanMainLandAsync(request));

        // Assert
        mockGmlHandler.VerifyAll();
        
        return Task.CompletedTask;
    }
}