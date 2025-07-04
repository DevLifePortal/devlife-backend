namespace DevLife.Infrastructure.Database.Redis.Github;

public interface IGitHubTokenStorage
{
    public Task SetTokenAsync(string userName, string token, CancellationToken cancellationToken);
    public Task<string?> GetTokenAsync(string userName, CancellationToken cancellationToken);
}