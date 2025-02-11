using System.Security.Claims;
using coords_to_taiwanese_city_country.Entities;
using coords_to_taiwanese_city_country.Utilities.Abstracts;
using Moq;
using Moq.AutoMock;
using Moq.EntityFrameworkCore;

namespace tests.Services.AuthService;

[TestFixture]
[Description("針對 AuthService 的刪除帳號方法的單元測試")]
public class AuthServiceDeleteTests
{
    [Test]
    [Description("驗證 DeleteAsync 應從 claims 中取得 GUID，藉此查詢帳號後將帳號從 DB 刪除")]
    public async Task DeleteAsync_ShouldFindAccountByGuidExtractedFromClaims_AndDeleteFromDatabase()
    {
        // Arrange
        AutoMocker autoMocker = new();

        Guid id = Guid.NewGuid();
        
        Claim claim = new(ClaimTypes.NameIdentifier, id.ToString());
        ClaimsIdentity claimsIdentity = new([claim]);
        ClaimsPrincipal claimsPrincipal = new(claimsIdentity);
        
        Mock<IJwtGuidHandler> mockGuidHandler = autoMocker.GetMock<IJwtGuidHandler>();
        mockGuidHandler
            .Setup(handler => handler.GetGuidFromClaims(claimsPrincipal.Claims))
            .Returns(id.ToString())
            .Verifiable(Times.Once);

        UserAccount mockData = new()
        {
            Id = id
        };
        
        Mock<DatabaseContext> mockContext = new();
        mockContext.Setup(ctx => ctx.UserAccounts)
            .ReturnsDbSet([mockData])
            .Verifiable(Times.Once);
        
        mockContext.Setup(ctx => ctx.Remove(mockData))
            .Verifiable(Times.Once);
        
        mockContext.Setup(ctx => ctx.SaveChangesAsync(default))
            .Verifiable(Times.Once);

        autoMocker.Use(mockContext);
        
        coords_to_taiwanese_city_country.Services.AuthService authService =
            autoMocker.Get<coords_to_taiwanese_city_country.Services.AuthService>();

        // Act
        await authService.DeleteAsync(claimsPrincipal);

        // Assert
        autoMocker.Verify();
    }
    
    [Test]
    [Description("驗證 DeleteAsync 應從 claims 中取得 GUID，如果取不到則直接忽略回傳")]
    public async Task DeleteAsync_ShouldTryExtractGuidFromClaims_AndReturnIfNull()
    {
        // Arrange
        AutoMocker autoMocker = new();
        autoMocker.Use(new Mock<DatabaseContext>());
        
        ClaimsIdentity claimsIdentity = new([]);
        ClaimsPrincipal claimsPrincipal = new(claimsIdentity);
        
        Mock<IJwtGuidHandler> mockGuidHandler = autoMocker.GetMock<IJwtGuidHandler>();
        mockGuidHandler
            .Setup(handler => handler.GetGuidFromClaims(claimsPrincipal.Claims))
            .Returns((string?)null)
            .Verifiable(Times.Once);
        
        coords_to_taiwanese_city_country.Services.AuthService authService =
            autoMocker.Get<coords_to_taiwanese_city_country.Services.AuthService>();

        // Act
        await authService.DeleteAsync(claimsPrincipal);

        // Assert
        autoMocker.Verify();
    }
    
    [Test]
    [Description("驗證 DeleteAsync 應從 claims 中取得 GUID 並查詢對應的使用者帳號，如果資料不存在則直接回傳")]
    public async Task DeleteAsync_ShouldTryExtractGuidFromClaimsAndQueryUserAccount_AndReturnIfNotFound()
    {
        // Arrange
        AutoMocker autoMocker = new();
        
        Guid id = Guid.NewGuid();
        
        Claim claim = new(ClaimTypes.NameIdentifier, id.ToString());
        ClaimsIdentity claimsIdentity = new([claim]);
        ClaimsPrincipal claimsPrincipal = new(claimsIdentity);
        
        Mock<IJwtGuidHandler> mockGuidHandler = autoMocker.GetMock<IJwtGuidHandler>();
        mockGuidHandler
            .Setup(handler => handler.GetGuidFromClaims(claimsPrincipal.Claims))
            .Returns(id.ToString())
            .Verifiable(Times.Once);
        
        Mock<DatabaseContext> mockContext = new();
        mockContext.Setup(ctx => ctx.UserAccounts)
            .ReturnsDbSet([])
            .Verifiable(Times.Once);
        
        autoMocker.Use(mockContext);
        
        coords_to_taiwanese_city_country.Services.AuthService authService =
            autoMocker.Get<coords_to_taiwanese_city_country.Services.AuthService>();

        // Act
        await authService.DeleteAsync(claimsPrincipal);

        // Assert
        autoMocker.Verify();
    }
}