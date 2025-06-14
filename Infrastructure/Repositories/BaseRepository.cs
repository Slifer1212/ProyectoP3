using System.Linq.Expressions;
using Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infraestructure.Repositories;

public class BaseRepository<TEntity, TKey> : IBaseRepository<TEntity, TKey> where TEntity : class
{
    protected readonly LibraryDbContext _context;
    protected readonly DbSet<TEntity> _dbSet;

    public BaseRepository(LibraryDbContext context)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
    }

    public async Task<TEntity?> GetByIdAsync(TKey id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await _dbSet.Where(predicate).ToListAsync();
    }

    public async Task<TEntity?> FindSingleAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await _dbSet.FirstOrDefaultAsync(predicate);
    }

    public async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await _dbSet.AnyAsync(predicate);
    }

    public async Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null)
    {
        return predicate == null 
            ? await _dbSet.CountAsync() 
            : await _dbSet.CountAsync(predicate);
    }

    public async Task AddAsync(TEntity entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public async Task AddRangeAsync(IEnumerable<TEntity> entities)
    {
        await _dbSet.AddRangeAsync(entities);
    }

    public void Update(TEntity entity)
    {
        _dbSet.Update(entity);
    }

    public void Remove(TEntity entity)
    {
        _dbSet.Remove(entity);
    }

    public void RemoveRange(IEnumerable<TEntity> entities)
    {
        _dbSet.RemoveRange(entities);
    }

    public IQueryable<TEntity> Query()
    {
        return _dbSet.AsQueryable();
    }

    public IQueryable<TEntity> QueryIncluding(params Expression<Func<TEntity, object>>[] includeProperties)
    {
        IQueryable<TEntity> query = _dbSet;
        
        foreach (var includeProperty in includeProperties)
        {
            query = query.Include(includeProperty);
        }
        
        return query;
    }
}