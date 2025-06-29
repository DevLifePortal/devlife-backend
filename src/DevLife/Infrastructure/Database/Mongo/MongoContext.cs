using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace DevLife.Infrastructure.Database.Mongo;

public class MongoContext
{
    private readonly IMongoDatabase _database;
    public IMongoDatabase Database => _database;

    public MongoClient Client { get; }

    public MongoContext(IOptions<MongoConfiguration> config)
    {
        var settings = config.Value;

        Client = new MongoClient(settings.ConnectionString);
        _database = Client.GetDatabase(settings.DatabaseName);

        CreateCollectionsIfNotExist(_database).GetAwaiter().GetResult();
    }

    private static async Task CreateCollectionsIfNotExist(IMongoDatabase db)
    {
        var existingCollections = await (await db.ListCollectionNamesAsync()).ToListAsync();

        if (!existingCollections.Contains("GameSessions"))
            await db.CreateCollectionAsync("GameSessions");

        if (!existingCollections.Contains("UserProfiles"))
            await db.CreateCollectionAsync("UserProfiles");
        
        if (!existingCollections.Contains("CodingTasks"))
            await db.CreateCollectionAsync("CodingTasks");
        
        if (!existingCollections.Contains("ProfileInteractions"))
            await db.CreateCollectionAsync("ProfileInteractions");
    }
}