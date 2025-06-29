using System.Linq.Expressions;

namespace DevLife.Infrastructure.Database.Postgres.Repository;

public interface IPostgresRepository<TEntity>
    where TEntity : class
{
    public Task AddAsync(TEntity entity, CancellationToken cancellationToken);
    
    public Task UpdateAsync(TEntity entity, CancellationToken cancellationToken);
    
    public Task DeleteAsync(TEntity[] entities, CancellationToken cancellationToken);
    
    public IQueryable<TEntity> Where(Expression<Func<TEntity, bool>> predicate);
    
    public IQueryable<TEntity> AsQueryable();
}