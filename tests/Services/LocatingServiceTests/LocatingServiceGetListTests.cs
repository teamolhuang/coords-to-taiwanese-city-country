using coords_to_taiwanese_city_country.Models;
using coords_to_taiwanese_city_country.Services;
using coords_to_taiwanese_city_country.Utilities.Abstracts;
using Moq;
using Moq.AutoMock;

namespace tests.Services.LocatingServiceTests;

[TestFixture]
[Description("針對 LocatingService 中取得完整列表方法的各種測試。")]
public class LocatingServiceGetListTests
{
    [Test]
    [Description("驗證 GetListAsync 應該要呼叫 gmlHandler，並把資料轉換成 API 的回傳格式。並且整個生命週期只會呼叫一次，後續會直接使用快取。")]
    public async Task GetListAsync_ShouldCallGmlHandlerOnlyWhenInit_ThenReturnOnlyCacheAfterThat()
    {
        // Arrange
        Random random = new();
        IEnumerable<string> mockResults = Enumerable.Range(0, random.Next(100, 200))
            .Select(_ => Guid.NewGuid().ToString())
            .ToArray();
        
        AutoMocker autoMocker = new();

        Mock<IGmlHandler> mockGmlHandler = autoMocker.GetMock<IGmlHandler>();
        
        mockGmlHandler.Setup(gml => gml.GetListAsync())
            .ReturnsAsync(mockResults)
            .Verifiable(Times.Once);
        
        LocatingService locatingService = autoMocker.Get<LocatingService>();
        
        // Act
        for (int i = 1; i <= random.Next(100, 2000); i++)
        {
            CityCountryListResponse response = await locatingService.GetListAsync();

            // Assert
            // 把轉換 mockResults 轉換成 前三字 : 後續字串 的 lookup，以便做 Assert
            ILookup<string, string> lookup = mockResults.ToLookup(x => new string(x.Take(3).ToArray()),
                x => new string(x.Skip(3).ToArray()));

            Assert.That(response.Cities.Select(c => c.Name), Is.EquivalentTo(lookup.Select(g => g.Key)));
            Assert.That(response.CityCount, Is.EqualTo(lookup.Count()));
            Assert.That(response.CountryCount, Is.EqualTo(lookup.SelectMany(g => g).Count()));

            foreach (CityCountryListResponseCity city in response.Cities)
            {
                Assert.That(city.Countries, Is.EquivalentTo(lookup[city.Name]));
                Assert.That(city.CountryCount, Is.EqualTo(lookup[city.Name].Count()));
            }

            autoMocker.Verify();
        }
    }
}