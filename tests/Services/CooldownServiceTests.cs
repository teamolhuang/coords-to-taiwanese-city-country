using coords_to_taiwanese_city_country.Services;
using coords_to_taiwanese_city_country.Utilities;
using Moq.AutoMock;

namespace tests.Services;

[TestFixture]
[Description("針對 CooldownService 的單元測試")]
public class CooldownServiceTests
{
    [Test]
    [Description("驗證 CheckCooldownAsync 方法應該在超出指定的限制之後每次都拋出，直到冷卻期間過了之後又能馬上再次使用")]
    public async Task CheckCooldownAsync_ShouldThrowOnFullWindow_AndThenRecoversAfterDuration()
    {
        // Arrange
        AutoMocker autoMocker = new();
        CooldownService cooldownService = autoMocker.Get<CooldownService>();

        Random random = new();
        TimeSpan duration = TimeSpan.FromMilliseconds(random.Next(500, 1500));
        int limit = random.Next(1, 3);

        Guid identifier = Guid.NewGuid();
        
        // Act & Assert
        // 1. 連續執行達 limit+1 次，前 limit 次預期都會成功，只有最後一次預期失敗
        // 2. 等到超過秒數範圍之後再執行一次，預期會成功

        DateTime windowShiftsOn = DateTime.Now.Add(duration);
        
        for (int i = 1; i <= limit; i++)
            Assert.DoesNotThrowAsync(async () => await cooldownService.CheckCooldownAsync(duration, limit, identifier.ToString()));
        
        Assert.ThrowsAsync<TooManyRequestsException>(async () => await cooldownService.CheckCooldownAsync(duration, limit, identifier.ToString()));

        while (DateTime.Now < windowShiftsOn)
            await Task.Delay(100);
        
        Assert.DoesNotThrowAsync(async () => await cooldownService.CheckCooldownAsync(duration, limit, identifier.ToString()));
    }
}