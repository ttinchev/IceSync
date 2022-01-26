namespace IceSync.Infrastructure.Models.Settings
{
    /// <summary>
    /// Application Settings.
    /// </summary>
    public class ApplicationSettings
    {
        /// <summary>
        /// Gets or sets a value indicating whether swagger is shown.
        /// </summary>
        public bool EnableSwagger { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the client location.
        /// </summary>
        public string ClientLocation { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the secret for encoding the token.
        /// </summary>
        public string JwtSecret { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the url of the api.
        /// </summary>
        public string WorkflowsAPI { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the company id.
        /// </summary>
        public string WorkflowsAPICompanyId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the user id.
        /// </summary>
        public string WorkflowsAPIUserId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the secret.
        /// </summary>
        public string WorkflowsAPIUserSecret { get; set; }
    }
}
