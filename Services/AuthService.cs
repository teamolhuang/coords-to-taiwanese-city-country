using System.Data;
using System.Security.Claims;
using coords_to_taiwanese_city_country.Entities;
using coords_to_taiwanese_city_country.Models;
using coords_to_taiwanese_city_country.Services.Abstracts;
using coords_to_taiwanese_city_country.Utilities;
using coords_to_taiwanese_city_country.Utilities.Abstracts;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

namespace coords_to_taiwanese_city_country.Services;

/// <inheritdoc />
public class AuthService(DatabaseContext databaseContext,
    IConfiguration configuration,
    IJwtTokenHelper jwtTokenHelper,
    IJwtGuidHandler jwtGuidHandler,
    IRedisContext redisContext) : IAuthService
{
    /// <inheritdoc />
    public async Task<RegisterResponse> RegisterAsync(RegisterRequest request)
    {
        // 1. 查詢是否已有相同的帳號

        bool isExist = await databaseContext.UserAccounts.AnyAsync(ua => ua.Account == request.Account);

        if (isExist)
            throw new DuplicateNameException($"帳號「{request.Account}」已存在");
        
        // 2. 把密碼加密以便後續寫入資料庫
        string? encryptedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);
        
        // 3. 建立新資料並寫入

        UserAccount newData = new()
        {
            Account = request.Account,
            HashedPassword = encryptedPassword
        };

        await databaseContext.AddAsync(newData);
        await databaseContext.SaveChangesAsync();

        // 4. 回傳
        return new RegisterResponse
        {
            Account = newData.Account
        };
    }

    /// <inheritdoc />
    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        // 1. 找出相同的帳號
        UserAccount data = await databaseContext.UserAccounts
                               .Where(ua => ua.BannedUntil == null || ua.BannedUntil < DateTime.Now)
                               .FirstOrDefaultAsync(ua => ua.Account == request.Account) 
                           ?? throw new NullReferenceException("帳號或密碼輸入錯誤");
        
        // 2. 驗證密碼
        bool isVerified = BCrypt.Net.BCrypt.Verify(request.Password, data.HashedPassword);
        
        if (!isVerified)
            throw new NullReferenceException("帳號或密碼輸入錯誤");
        
        // 3. 發行 JWT 並回傳
        string privateKey = configuration.GetSection("Jwt").GetValue<string>("SigningKey") ?? throw new NullReferenceException("SingingKey missed!");
        
        LoginResponse result = new()
        {
            AccessToken = jwtTokenHelper.GenerateToken(data.Id, privateKey),
            ExpiresAt = DateTime.Now.Add(jwtTokenHelper.GetExpirationSpan())
        };

        return result;
    }
    
    /// <inheritdoc />
    public async Task DeleteAsync(ClaimsPrincipal claimsPrincipal)
    {
        // 1. 找出帳號，如果已不存在，當成已刪除成功並回傳
        // 本系統固定會把 GUID 放在 claims 裡面
        string? userId = jwtGuidHandler.GetGuidFromClaims(claimsPrincipal.Claims);

        if (userId is null)
            return;

        Guid userIdGuid = new(userId);
        
        UserAccount? data = await FindUserAccountByIdAsync(userIdGuid);

        if (data is null)
            return;

        // 2. 執行刪除
        databaseContext.Remove(data);
        await databaseContext.SaveChangesAsync();
        
        // 3. 把刪除的 GUID 存進 Redis，並更新過期時間為本系統的 JWT 有效時間
        IDatabase database = redisContext.Database;
        await database.SetAddAsync(RedisContextKeys.DeletedAccount, userId);
        await database.KeyExpireAsync(RedisContextKeys.DeletedAccount, jwtTokenHelper.GetExpirationSpan());
    }

    private async Task<UserAccount?> FindUserAccountByIdAsync(Guid id)
    {
        UserAccount? data = await databaseContext.UserAccounts
            .FirstOrDefaultAsync(ua => ua.Id == id);
        return data;
    }

    /// <inheritdoc />
    public async Task BanAsync(string userIdentifier, TimeSpan duration)
    {
        UserAccount? userData = await FindUserAccountByIdAsync(new Guid(userIdentifier));
        
        // 如果帳號已不存在，忽略並返回
        if (userData is null)
            return;

        // 如果還在禁閉期間，就加時到原本的禁用期限上
        DateTime start = DateTime.Now <= userData.BannedUntil
            ? userData.BannedUntil.Value
            : DateTime.Now;

        userData.BannedUntil = start.Add(duration);
        
        await databaseContext.SaveChangesAsync();
    }
}