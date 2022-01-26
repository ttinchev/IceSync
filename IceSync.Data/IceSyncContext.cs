using IceSync.Data.Configuration;
using IceSync.Infrastructure.Domain;

using Microsoft.EntityFrameworkCore;

namespace IceSync.Data
{
    /// <summary>
    /// IceSync Database Contextclass.
    /// </summary>
    public class IceSyncContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IceSyncContext"/> class.
        /// </summary>
        /// <param name="options">Context options.</param>
        public IceSyncContext(DbContextOptions<IceSyncContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Gets or sets Workflows.
        /// </summary>
        public virtual DbSet<Workflow> Workflows { get; set; }

        /// <summary>
        /// Gets or sets Token.
        /// </summary>
        public virtual DbSet<Token> Token { get; set; }

        /// <inheritdoc/>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new WorkflowConfiguration());
            modelBuilder.ApplyConfiguration(new TokenConfiguration());
        }
    }
}
