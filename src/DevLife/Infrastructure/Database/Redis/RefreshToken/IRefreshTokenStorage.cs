namespace DevLife.Infrastructure.Database.Redis.RefreshToken;

public interface IRefreshTokenStorage
{ 
    public Task SetAsync(string userName, string refreshToken, TimeSpan expiresAt, CancellationToken cancellationToken);
    public Task<string?> GetAsync(string userName, CancellationToken cancellationToken);
    public Task RemoveAsync(string userName, CancellationToken cancellationToken);
}