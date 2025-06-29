using System.Linq.Expressions;
using DevLife.Infrastructure.Models.Abstractions;
using MongoDB.Driver;

namespace DevLife.Infrastructure.Database.Mongo.Repository;

public class MongoRepository<TEntity> : IMongoRepository<TEntity> where TEntity : IMongoEntity
{
    private readonly IMongoCollection<TEntity> _collection;

    public MongoRepository(MongoContext context)
    {
        _collection = context.Database.GetCollection<TEntity>(typeof(TEntity).Name + "s");
    }

    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(entity);
        await _collection.InsertOneAsync(entity, cancellationToken: cancellationToken);
    }

    public async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(entity);
        await _collection.ReplaceOneAsync(x => x.Id == entity.Id, entity, cancellationToken: cancellationToken);
    }

    public async Task DeleteAsync(TEntity[] entities, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(entities);
        var ids = entities.Select(e => e.Id).ToArray();
        await _collection.DeleteManyAsync(x => ids.Contains(x.Id), cancellationToken);
    }

    public IQueryable<TEntity> AsQueryable()
    {
        return _collection.AsQueryable();
    }

    public IQueryable<TEntity> Where(Expression<Func<TEntity, bool>> predicate)
    {
        return _collection.AsQueryable().Where(predicate);
    }
}