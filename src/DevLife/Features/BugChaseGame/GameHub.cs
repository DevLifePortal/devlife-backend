using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;

namespace DevLife.Features.BugChaseGame;

public class GameHub : Hub
{
    private readonly GameSessionManager _sessionManager;
    private readonly LeaderboardService _leaderboard;

    public GameHub(GameSessionManager sessionManager, LeaderboardService leaderboard)
    {
        _sessionManager = sessionManager;
        _leaderboard = leaderboard;
    }

    public override async Task OnConnectedAsync()
    {
        var userName = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userName is null) return;

        _sessionManager.AddPlayer(userName);

        var others = _sessionManager
            .GetAll()
            .Where(p => p.UserName != userName);
        await Clients.Caller.SendAsync("OtherPlayersState", others);
        
        var user = _sessionManager.GetPlayer(userName);
        
        if (user is not null)
            await Clients.Others.SendAsync("PlayerJoined", user);
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        var username = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (username is null) 
            return Task.CompletedTask;

        _sessionManager.RemovePlayer(username);
        return Clients.All.SendAsync("PlayerLeft", username);
    }
    public async Task JumpAsync()
    {
        var username = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (username is null)
            return;

        await Clients.All.SendAsync("PlayerJumped", username);
    }

    public async Task DuckAsync()
    {
        var username = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (username is null)
            return;
        
        await Clients.All.SendAsync("PlayerDucked", username);
    }

    public async Task UpdateScoreAsync(int score)
    {
        var username = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (username is null)
            return;

        _sessionManager.UpdateScore(username, score);
        _leaderboard.UpdateScore(username, score);

        var topPlayers = _leaderboard.GetTopPlayers(5);
        await Clients.All.SendAsync("LeaderboardUpdated", topPlayers);
    }
    
    public async Task UpdateY(float y)
    {
        var userName = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userName is null) 
            return;

        _sessionManager.UpdateY(userName, y);
        await Clients.Others.SendAsync("PlayerYUpdated", userName, y);
    }


    public async Task GameOverAsync()
    {
        var username = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (username is null)
            return;
        
        var user = _sessionManager.GetPlayer(username);
        if (user is null || user.IsAlive == false)
            return;
        
        _sessionManager.MarkAsDead(username);
        await Clients.All.SendAsync("PlayerDied", username);
    }
}