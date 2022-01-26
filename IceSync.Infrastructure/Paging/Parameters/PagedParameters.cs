namespace IceSync.Infrastructure.Paging.Parameters
{
    /// <summary>
    /// Paged parameters class.
    /// </summary>
    public class PagedParameters
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PagedParameters"/> class.
        /// </summary>
        public PagedParameters()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PagedParameters"/> class.
        /// </summary>
        public PagedParameters(int take, int skip, bool sortAsc, string sortBy)
        {
            Take = take;
            Skip = skip;
            SortAsc = sortAsc;
            SortBy = sortBy;
        }

        /// <summary>Gets or sets the number of skipped items.</summary>
        public int Skip { get; set; } = 0;

        /// <summary>Gets or sets the number of items to be returned.</summary>
        public int Take { get; set; } = 50;

        /// <summary>Gets or sets the sort key.</summary>
        public string SortBy { get; set; }

        /// <summary>Gets or sets a value indicating whether the sort direction is ascending.</summary>
        public bool SortAsc { get; set; }
    }
}
