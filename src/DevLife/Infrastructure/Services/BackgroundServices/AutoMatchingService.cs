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

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            using var scope = _serviceProvider.CreateScope();

            var interactions = scope.ServiceProvider.GetRequiredService<IMongoRepository<ProfileInteraction>>()
                .AsQueryable()
                .ToList();

            var matchRepository = scope.ServiceProvider.GetRequiredService<IMongoRepository<Match>>();
            var existingMatches = matchRepository.AsQueryable().ToList();

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

                await matchRepository.AddAsync(match, cancellationToken);
            }

            await Task.Delay(_interval, cancellationToken);
        }
    }
}