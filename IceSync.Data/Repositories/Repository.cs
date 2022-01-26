using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

using IceSync.Infrastructure.Repositories;

using Microsoft.EntityFrameworkCore;

#pragma warning disable CS1591
#pragma warning disable SA1600
#pragma warning disable SA1401

namespace IceSync.Data.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity>
        where TEntity : class
    {
        protected IceSyncContext _context;

        public Repository(IceSyncContext context)
        {
            _context = context;
        }

        public virtual async Task<TEntity> AddAsync(TEntity t)
        {
            _context.Set<TEntity>().Add(t);
            await _context.SaveChangesAsync();
            return t;
        }

        public virtual async Task<ICollection<TEntity>> AddManyAsync(ICollection<TEntity> t)
        {
            await _context.Set<TEntity>().AddRangeAsync(t);
            await _context.SaveChangesAsync();
            return t;
        }

        public virtual async Task<TEntity> UpdateAsync(TEntity t)
        {
            _context.Set<TEntity>().Update(t);
            await _context.SaveChangesAsync();
            return t;
        }

        public virtual async Task<ICollection<TEntity>> UpdateManyAsync(ICollection<TEntity> t)
        {
            _context.Set<TEntity>().UpdateRange(t);
            await _context.SaveChangesAsync();
            return t;
        }

        public virtual async Task<int> DeleteAsync(TEntity entity)
        {
            _context.Set<TEntity>().Remove(entity);
            return await SaveChangesAsync();
        }

        public virtual async Task<int> DeleteManyAsync(ICollection<TEntity> t)
        {
            _context.Set<TEntity>().RemoveRange(t);
            return await SaveChangesAsync();
        }

        public virtual async Task<TEntity> GetAsync(object key)
        {
            return await _context.Set<TEntity>().FindAsync(key);
        }

        public IQueryable<TEntity> GetAll()
        {
            return _context.Set<TEntity>().AsNoTracking();
        }

        public IQueryable<TEntity> GetAllTracking()
        {
            return _context.Set<TEntity>();
        }

        public virtual async Task<IReadOnlyList<TEntity>> GetAllAsync()
        {
            var res = await _context.Set<TEntity>().AsNoTracking().ToListAsync();
            return res;
        }

        public virtual async Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> match, bool noTracking = true)
        {
            return noTracking
                ? await _context.Set<TEntity>().AsNoTracking().SingleOrDefaultAsync(match)
                : await _context.Set<TEntity>().SingleOrDefaultAsync(match);
        }

        public virtual async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task<int> CountAllByAsync(Expression<Func<TEntity, bool>> condition)
        {
            return await _context.Set<TEntity>().Where(condition).CountAsync();
        }
    }
}
