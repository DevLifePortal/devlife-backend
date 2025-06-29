namespace DevLife.Features.BugChaseGame;

public record Player(string UserName, int Score, bool IsAlive, float Y);

public record PlayerState(string UserName, bool IsAlive, float Y); 
