using System.Linq.Expressions;
using DevLife.Infrastructure.Models.Abstractions;

namespace DevLife.Infrastructure.Database.Mongo.Repository;

public interface IMongoRepository<TEntity>
    where TEntity : IMongoEntity
{
    Task AddAsync(TEntity entity, CancellationToken cancellationToken);
    Task UpdateAsync(TEntity entity, CancellationToken cancellationToken);
    Task DeleteAsync(TEntity[] entities, CancellationToken cancellationToken);
    IQueryable<TEntity> Where(Expression<Func<TEntity, bool>> predicate);
    IQueryable<TEntity> AsQueryable();
    
    
}