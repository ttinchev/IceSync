namespace IceSync.Infrastructure.Results
{
    /// <summary>
    /// Metadata for the result object.
    /// </summary>
    public class ResultMetadata : IResultMetadata
    {
        /// <summary>
        /// Gets or sets the number of elements that are skipped.
        /// </summary>
        public int Skip { get; set; }

        /// <summary>
        /// Gets or sets the number of elements that are returned.
        /// </summary>
        public int Take { get; set; }

        /// <summary>
        /// Gets or sets the total number of elements for this query.
        /// </summary>
        public int TotalCount { get; set; }
    }
}
