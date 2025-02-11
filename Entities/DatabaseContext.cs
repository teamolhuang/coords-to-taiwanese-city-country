using Microsoft.EntityFrameworkCore;

namespace coords_to_taiwanese_city_country.Entities;

/// <summary>
/// 資料庫
/// </summary>
public class DatabaseContext : DbContext
{
    /// <inheritdoc />
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
    }

    /// <inheritdoc />
    public DatabaseContext()
    {
    }
    
    /// <summary>
    /// 使用者帳號
    /// </summary>
    public virtual DbSet<UserAccount> UserAccounts { get; set; }
}