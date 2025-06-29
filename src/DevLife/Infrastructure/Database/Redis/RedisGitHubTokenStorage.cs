using StackExchange.Redis;

namespace DevLife.Infrastructure.Database.Redis;

public class RedisGitHubTokenStorage : IGitHubTokenStorage
{
    private readonly IDatabase _db;
    private readonly TimeSpan _ttl = TimeSpan.FromDays(7);

    public RedisGitHubTokenStorage(IConnectionMultiplexer redis)
    {
        _db = redis.GetDatabase();
    }

    public Task SetTokenAsync(string userName, string token, CancellationToken cancellationToken)
        => _db.StringSetAsync($"github:token:{userName}", token, _ttl);

    public async Task<string?> GetTokenAsync(string userName, CancellationToken cancellationToken)
    {
        var token = await _db.StringGetAsync($"github:token:{userName}");
        return token.HasValue ? token.ToString() : null;
    }
}