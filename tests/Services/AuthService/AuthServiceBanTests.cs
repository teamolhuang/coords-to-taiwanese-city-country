using coords_to_taiwanese_city_country.Entities;
using Moq;
using Moq.AutoMock;
using Moq.EntityFrameworkCore;

namespace tests.Services.AuthService;

[TestFixture]
[Description("針對 AuthService 中禁用帳號方法的單元測試")]
public class AuthServiceBanTests
{
    [Test]
    [Description("驗證 BanAsync 應查出使用者帳號之後，依據傳入的時間長度設定該帳號的 BannedUntil")]
    public async Task BanAsync_ShouldFindUserAccount_ThenSetBannedUntilAndSaveChanges()
    {
        // Arrange
        DateTime start = DateTime.Now;
        
        AutoMocker autoMocker = new();
        
        Guid mockGuid = Guid.NewGuid();
        int mockBanMinutes = new Random().Next(1, 100);

        Mock<DatabaseContext> mockContext = new(); 
        
        UserAccount mockData = new()
        {
            Id = mockGuid,
            BannedUntil = null
        };

        mockContext.Setup(ctx => ctx.UserAccounts)
            .ReturnsDbSet([mockData])
            .Verifiable(Times.Once);
        
        mockContext.Setup(ctx => ctx.SaveChangesAsync(default))
            .Verifiable(Times.Once);

        autoMocker.Use(mockContext);
        
        coords_to_taiwanese_city_country.Services.AuthService authService = autoMocker.Get<coords_to_taiwanese_city_country.Services.AuthService>();
        
        // Act
        await authService.BanAsync(mockGuid.ToString(), TimeSpan.FromMinutes(mockBanMinutes));

        // Assert
        Assert.That(mockData.BannedUntil, Is.GreaterThanOrEqualTo(start.AddMinutes(mockBanMinutes)));
        autoMocker.Verify();
    }
    
    [Test]
    [Description("驗證 BanAsync 應查出使用者帳號之後，在該帳號已經處於封鎖期間時，依據傳入的時間長度更新該帳號的 BannedUntil")]
    public async Task BanAsync_ShouldFindUserAccountAlreadyBanned_ThenUpdateBannedUntilAndSaveChanges()
    {
        // Arrange
        AutoMocker autoMocker = new();
        
        Guid mockGuid = Guid.NewGuid();
        Random random = new();
        int mockBanMinutes = random.Next(1, 100);

        Mock<DatabaseContext> mockContext = new(); 
        
        DateTime originalBannedUntil = DateTime.Now.AddMinutes(random.Next());
        UserAccount mockData = new()
        {
            Id = mockGuid,
            BannedUntil = originalBannedUntil
        };

        mockContext.Setup(ctx => ctx.UserAccounts)
            .ReturnsDbSet([mockData])
            .Verifiable(Times.Once);
        
        mockContext.Setup(ctx => ctx.SaveChangesAsync(default))
            .Verifiable(Times.Once);

        autoMocker.Use(mockContext);
        
        coords_to_taiwanese_city_country.Services.AuthService authService = autoMocker.Get<coords_to_taiwanese_city_country.Services.AuthService>();
        
        // Act
        await authService.BanAsync(mockGuid.ToString(), TimeSpan.FromMinutes(mockBanMinutes));

        // Assert
        Assert.That(mockData.BannedUntil, Is.EqualTo(originalBannedUntil.AddMinutes(mockBanMinutes)));
        autoMocker.Verify();
    }
    
    [Test]
    [Description("驗證 BanAsync 應在查無使用者帳號時，忽略並回傳")]
    public async Task BanAsync_ShouldIgnoreNotFoundAccount_ThenReturn()
    {
        // Arrange
        AutoMocker autoMocker = new();
        
        Guid mockGuid = Guid.NewGuid();
        int mockBanMinutes = new Random().Next(1, 100);

        Mock<DatabaseContext> mockContext = new(); 
        
        mockContext.Setup(ctx => ctx.UserAccounts)
            .ReturnsDbSet([])
            .Verifiable(Times.Once);
        
        autoMocker.Use(mockContext);
        
        coords_to_taiwanese_city_country.Services.AuthService authService = autoMocker.Get<coords_to_taiwanese_city_country.Services.AuthService>();
        
        // Act
        await authService.BanAsync(mockGuid.ToString(), TimeSpan.FromMinutes(mockBanMinutes));

        // Assert
        autoMocker.Verify();
    }
}