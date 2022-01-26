using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

#pragma warning disable CS1591
#pragma warning disable SA1600

namespace IceSync.Infrastructure.Repositories
{
    /// <summary>
    /// Generic repository interface.
    /// </summary>
    /// <typeparam name="T"> Entity class.</typeparam>
    public interface IRepository<T>
        where T : class
    {
        Task<T> AddAsync(T t);

        Task<ICollection<T>> AddManyAsync(ICollection<T> t);

        Task<T> UpdateAsync(T t);

        Task<ICollection<T>> UpdateManyAsync(ICollection<T> t);

        Task<int> DeleteAsync(T entity);

        Task<int> DeleteManyAsync(ICollection<T> t);

        Task<T> GetAsync(object key);

        IQueryable<T> GetAll();

        IQueryable<T> GetAllTracking();

        Task<IReadOnlyList<T>> GetAllAsync();

        Task<T> FindAsync(Expression<Func<T, bool>> match, bool noTracking = true);

        Task<int> SaveChangesAsync();

        Task<int> CountAllByAsync(Expression<Func<T, bool>> condition);
    }
}
