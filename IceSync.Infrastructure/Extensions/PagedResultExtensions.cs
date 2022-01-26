using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using IceSync.Infrastructure.Paging;
using IceSync.Infrastructure.Paging.Parameters;

using Microsoft.EntityFrameworkCore;

namespace IceSync.Infrastructure.Extensions
{
    /// <summary>
    /// Extension for paging collections.
    /// </summary>
    public static class PagedResultExtensions
    {
        /// <summary>
        /// Gets the collections for the current page.
        /// </summary>
        /// <typeparam name="T">Generic entity.</typeparam>
        /// <param name="query">Search pattern.</param>
        /// <param name="pagedParameters">Paging parameters.</param>
        /// <returns>Collection for the current page.</returns>
        public static PagedResult<T> GetPaged<T>(this IQueryable<T> query, PagedParameters pagedParameters)
        {
            return new PagedResult<T>
            {
                Results = query.Skip(pagedParameters.Skip).Take(pagedParameters.Take).ToList(),
                TotalCount = query.Count(),
                Skip = pagedParameters.Skip,
                Take = pagedParameters.Take,
            };
        }

        /// <summary>
        /// Gets the collections for the current page.
        /// </summary>
        /// <typeparam name="T">Generic entity.</typeparam>
        /// <param name="query">Search pattern.</param>
        /// <param name="pagedParameters">Paging parameters.</param>
        /// <returns>Collection for the current page.</returns>
        public static async Task<PagedResult<T>> GetPagedAsync<T>(this IQueryable<T> query, PagedParameters pagedParameters)
        {
            return new PagedResult<T>
            {
                Results = await query.Skip(pagedParameters.Skip).Take(pagedParameters.Take).ToListAsync(),
                TotalCount = await query.CountAsync(),
                Skip = pagedParameters.Skip,
                Take = pagedParameters.Take,
            };
        }

        /// <summary>
        /// Gets the collections for the current page.
        /// </summary>
        /// <typeparam name="T">Generic entity.</typeparam>
        /// <param name="query">Search pattern.</param>
        /// <param name="pagedParameters">Paging parameters.</param>
        /// <returns>Collection for the current page.</returns>
        public static async Task<PagedResult<T>> GetPagedAsync<T>(this Task<List<T>> query, PagedParameters pagedParameters)
        {
            var items = await query;

            return new PagedResult<T>
            {
                Results = items.Skip(pagedParameters.Skip).Take(pagedParameters.Take).ToList(),
                TotalCount = query.Result.Count,
                Skip = pagedParameters.Skip,
                Take = pagedParameters.Take,
            };
        }
    }
}
