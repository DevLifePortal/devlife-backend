namespace DevLife.Features.BugChaseGame;

public class LeaderboardService
{
    private readonly Dictionary<string, int> _scores = new();

    public void UpdateScore(string id, int score)
    {
        _scores[id] = score;
    }

    public List<Player> GetTopPlayers(int count)
    {
        return _scores
            .OrderByDescending(kv => kv.Value)
            .Take(count)
            .Select(kv => new Player(kv.Key, kv.Value, true, 0))
            .ToList();
    }
}