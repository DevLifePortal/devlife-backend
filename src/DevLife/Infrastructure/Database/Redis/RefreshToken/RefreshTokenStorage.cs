using StackExchange.Redis;

namespace DevLife.Infrastructure.Database.Redis.RefreshToken;

public class RefreshTokenStorage : IRefreshTokenStorage
{
    private readonly IDatabase _db;

    public RefreshTokenStorage(IConnectionMultiplexer redis)
    {
        _db = redis.GetDatabase();
    }
    
    public async Task SetAsync(string userName, string refreshToken, TimeSpan expiresAt, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(userName, refreshToken);
        await _db.StringSetAsync($"refresh:{userName}", refreshToken, expiresAt);
    }

    public async Task<string?> GetAsync(string userName, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(userName);
        return await _db.StringGetAsync($"refresh:{userName}");
    }

    public async Task RemoveAsync(string userName, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(userName);
        await _db.KeyDeleteAsync($"refresh:{userName}");
    }
}