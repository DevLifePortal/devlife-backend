namespace DevLife.Features.BugChaseGame;

public class GameSessionManager
{
    private readonly Dictionary<string, Player> _players = new();

    public void AddPlayer(string userName) =>
        _players[userName] = new Player(userName, 0, true, 0);

    public void UpdateScore(string userName, int score)
    {
        if (_players.TryGetValue(userName, out var player))
            _players[userName] = player with { Score = score };
    }

    public void MarkAsDead(string userName)
    {
        if (_players.TryGetValue(userName, out var player))
            _players[userName] = player with { IsAlive = false };
    }

    public void UpdateY(string userName, float y)
    {
        if (_players.TryGetValue(userName, out var player))
            _players[userName] = player with { Y = y };
    }

    public void RemovePlayer(string userName) =>
        _players.Remove(userName);
    

    public Player? GetPlayer(string userName) => 
        _players.GetValueOrDefault(userName);
    
    public IReadOnlyCollection<Player> GetAll() => _players.Values;
}