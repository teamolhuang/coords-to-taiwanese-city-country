using coords_to_taiwanese_city_country.Entities;
using coords_to_taiwanese_city_country.Models;
using coords_to_taiwanese_city_country.Utilities.Abstracts;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.AutoMock;
using Moq.EntityFrameworkCore;

namespace tests.Services.AuthService;

[TestFixture]
[Description("針對 AuthService 中登入方法的單元測試")]
public class AuthServiceLoginTests
{
    [Test]
    [Description("驗證 LoginAsync 應該查出帳號資料並驗證密碼，都成功後發行 JWT 並連同有效期限一起回傳")]
    public async Task LoginAsync_ShouldFindAccountAndVerifyPasswordAndCreateJwtToken_ThenReturnJwtTokenWithExpiration()
    {
        // Arrange
        LoginRequest request = new()
        {
            Account = Guid.NewGuid().ToString(),
            Password = Guid.NewGuid().ToString()
        };
        UserAccount mockData = new()
        {
            Id = Guid.NewGuid(),
            Account = request.Account,
            HashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password),
            BannedUntil = null
        };
        string mockToken = Guid.NewGuid().ToString();
        string mockSigningKey = Guid.NewGuid().ToString();
        int mockExpireMinutes = new Random().Next();
        
        AutoMocker autoMocker = new();

        Mock<DatabaseContext> mockContext = new();
        mockContext.Setup(ctx => ctx.UserAccounts)
            .ReturnsDbSet([mockData])
            .Verifiable(Times.Once);

        autoMocker.Use(mockContext);
        
        Dictionary<string, string?> mockConfigurationDict = new()
        {
            {"Jwt:SigningKey", mockSigningKey},
            {"Jwt:ExpireMinutes", mockExpireMinutes.ToString()}
        };

        IConfigurationRoot mockConfiguration = new ConfigurationBuilder()
            .AddInMemoryCollection(mockConfigurationDict)
            .Build();

        autoMocker.Use<IConfiguration>(mockConfiguration);

        Mock<IJwtTokenHelper> mockJwtTokenHelper = autoMocker.GetMock<IJwtTokenHelper>();
        mockJwtTokenHelper
            .Setup(jwt => jwt.GenerateToken(mockData.Id, mockSigningKey))
            .Returns(mockToken)
            .Verifiable(Times.Once);
        
        coords_to_taiwanese_city_country.Services.AuthService authService = autoMocker.Get<coords_to_taiwanese_city_country.Services.AuthService>();

        // Act
        LoginResponse result = await authService.LoginAsync(request);

        // Assert
        Assert.That(result.AccessToken, Is.EqualTo("Bearer " + mockToken));
        Assert.That(result.ExpiresAt, Is.LessThanOrEqualTo(DateTime.Now.AddMinutes(mockExpireMinutes)));
        autoMocker.Verify();
    }
    
    [Test]
    [Description("驗證 LoginAsync 應該在查無帳號資料時，拋出 NullReferenceException")]
    public Task LoginAsync_ShouldQueryAccountFirst_AndThrowNullReferenceExceptionOnNotFound()
    {
        // Arrange
        LoginRequest request = new()
        {
            Account = Guid.NewGuid().ToString(),
            Password = Guid.NewGuid().ToString()
        };
        
        AutoMocker autoMocker = new();

        Mock<DatabaseContext> mockContext = new();
        mockContext.Setup(ctx => ctx.UserAccounts)
            .ReturnsDbSet([])
            .Verifiable(Times.Once);

        autoMocker.Use(mockContext);
        
        coords_to_taiwanese_city_country.Services.AuthService authService = autoMocker.Get<coords_to_taiwanese_city_country.Services.AuthService>();

        // Act & Assert
        Assert.ThrowsAsync<NullReferenceException>(() => authService.LoginAsync(request));
        autoMocker.Verify();
        return Task.CompletedTask;
    }
    
    [Test]
    [Description("驗證 LoginAsync 應該在帳號被禁用時，拋出 NullReferenceException")]
    public Task LoginAsync_ShouldQueryAccountFirst_AndThrowNullReferenceExceptionOnBanned()
    {
        // Arrange
        LoginRequest request = new()
        {
            Account = Guid.NewGuid().ToString(),
            Password = Guid.NewGuid().ToString()
        };
        UserAccount mockData = new()
        {
            Id = Guid.NewGuid(),
            Account = request.Account,
            HashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password),
            BannedUntil = DateTime.Now.AddMinutes(new Random().Next(1, 100))
        };
        
        AutoMocker autoMocker = new();

        Mock<DatabaseContext> mockContext = new();
        mockContext.Setup(ctx => ctx.UserAccounts)
            .ReturnsDbSet([mockData])
            .Verifiable(Times.Once);

        autoMocker.Use(mockContext);
        
        coords_to_taiwanese_city_country.Services.AuthService authService = autoMocker.Get<coords_to_taiwanese_city_country.Services.AuthService>();

        // Act & Assert
        Assert.ThrowsAsync<NullReferenceException>(() => authService.LoginAsync(request));
        autoMocker.Verify();
        return Task.CompletedTask;
    }
}