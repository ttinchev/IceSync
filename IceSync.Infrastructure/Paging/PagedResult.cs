using System.Collections.Generic;

using IceSync.Infrastructure.Results;

namespace IceSync.Infrastructure.Paging
{
    /// <summary>
    /// Generic result for paged collections.
    /// </summary>
    /// <typeparam name="T">Generic class.</typeparam>
    public class PagedResult<T> : ResultMetadata
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PagedResult{T}"/> class.
        /// </summary>
        public PagedResult()
        {
            Results = new List<T>();
        }

        /// <summary>
        /// Gets or sets generic result.
        /// </summary>
        public IReadOnlyList<T> Results { get; set; }
    }
}
