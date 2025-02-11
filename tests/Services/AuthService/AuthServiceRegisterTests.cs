using System.Data;
using coords_to_taiwanese_city_country.Entities;
using coords_to_taiwanese_city_country.Models;
using Moq;
using Moq.AutoMock;
using Moq.EntityFrameworkCore;

namespace tests.Services.AuthService;

[TestFixture]
[Description("針對 AuthService 中註冊方法的單元測試")]
public class AuthServiceRegisterTests
{
    [Test]
    [Description("驗證 RegisterAsync 方法應該要檢查無重複帳號後，將使用者輸入的密碼透過 BCrypt 雜湊，然後回傳所註冊的新帳號。")]
    public async Task RegisterAsync_ShouldCheckNoSameAccountAndSaveDataWithBCryptPassword_ThenReturnAccount()
    {
        // Arrange
        RegisterRequest request = new()
        {
            Account = Guid.NewGuid().ToString(),
            Password = Guid.NewGuid().ToString()
        };
        
        Mock<DatabaseContext> mockedContext = new();

        mockedContext.Setup(ctx => ctx.UserAccounts)
            .ReturnsDbSet([])
            .Verifiable(Times.Once);

        mockedContext.Setup(ctx => ctx.AddAsync(
                It.Is<UserAccount>(ua
                    => ua.Account == request.Account
                       && BCrypt.Net.BCrypt.Verify(request.Password, ua.HashedPassword, default, default)
                ),
                default)
            )
            .Verifiable(Times.Once);
        
        mockedContext.Setup(ctx => ctx.SaveChangesAsync(default))
            .Verifiable(Times.Once);

        AutoMocker autoMocker = new();
        autoMocker.Use(mockedContext);
        coords_to_taiwanese_city_country.Services.AuthService authService = autoMocker.Get<coords_to_taiwanese_city_country.Services.AuthService>();
        
        // Act
        RegisterResponse result = await authService.RegisterAsync(request);

        // Assert
        Assert.That(result.Account, Is.EqualTo(request.Account));
        mockedContext.Verify();
    }
    
    [Test]
    [Description("驗證 RegisterAsync 方法應該要檢查重複帳號，當重複時拋出 DuplicateNameException。")]
    public Task RegisterAsync_ShouldCheckSameAccount_ThenThrowDuplicateNameException()
    {
        // Arrange
        RegisterRequest request = new()
        {
            Account = Guid.NewGuid().ToString(),
            Password = Guid.NewGuid().ToString()
        };
        
        Mock<DatabaseContext> mockedContext = new();

        mockedContext.Setup(ctx => ctx.UserAccounts)
            .ReturnsDbSet([new UserAccount
                {
                    Account = request.Account
                }
            ])
            .Verifiable(Times.Once);

        AutoMocker autoMocker = new();
        autoMocker.Use(mockedContext);
        coords_to_taiwanese_city_country.Services.AuthService authService = autoMocker.Get<coords_to_taiwanese_city_country.Services.AuthService>();
        
        // Act & Assert
        Assert.ThrowsAsync<DuplicateNameException>(async () => await authService.RegisterAsync(request));
        
        return Task.CompletedTask;
    }
}