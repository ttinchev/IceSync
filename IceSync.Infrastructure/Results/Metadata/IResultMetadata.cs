namespace IceSync.Infrastructure.Results
{
    /// <summary>
    /// Interface for the metadata object of result.
    /// </summary>
    public interface IResultMetadata
    {
        /// <summary>
        /// Gets given total count of metadata.
        /// </summary>
        public int TotalCount { get; }

        /// <summary>
        /// Gets the skip property.
        /// </summary>
        public int Skip { get; }

        /// <summary>
        /// Gets the take property.
        /// </summary>
        public int Take { get; }
    }
}
