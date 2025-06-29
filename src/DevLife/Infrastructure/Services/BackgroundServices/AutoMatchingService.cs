using DevLife.Infrastructure.Database.Mongo.Repository;
using DevLife.Infrastructure.Models.Entities;

namespace DevLife.Infrastructure.Services.BackgroundServices;

public class AutoMatchingService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly TimeSpan _interval = TimeSpan.FromMinutes(2);

    public AutoMatchingService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _serviceProvider.CreateScope();

            var interactions = scope.ServiceProvider.GetRequiredService<IMongoRepository<ProfileInteraction>>()
                .AsQueryable()
                .ToList();

            var matchRepo = scope.ServiceProvider.GetRequiredService<IMongoRepository<Match>>();
            var existingMatches = matchRepo.AsQueryable().ToList();

            var matchedPairs = interactions
                .Select(i => new { From = i.SourceProfileId, To = i.TargetProfileId })
                .Where(pair =>
                    interactions.Any(x => x.SourceProfileId == pair.To && x.TargetProfileId == pair.From) &&
                    !existingMatches.Any(m =>
                        (m.Profile1Id == pair.From && m.Profile2Id == pair.To) ||
                        (m.Profile1Id == pair.To && m.Profile2Id == pair.From)))
                .Distinct()
                .ToList();

            foreach (var pair in matchedPairs)
            {
                var match = new Match
                {
                    Profile1Id = pair.From,
                    Profile2Id = pair.To
                };

                await matchRepo.AddAsync(match, stoppingToken);
            }

            await Task.Delay(_interval, stoppingToken);
        }
    }
}