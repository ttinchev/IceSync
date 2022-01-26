using IceSync.Infrastructure.Domain;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

#pragma warning disable CS1591
#pragma warning disable SA1600

namespace IceSync.Data.Configuration
{
    public class WorkflowConfiguration : IEntityTypeConfiguration<Workflow>
    {
        public void Configure(EntityTypeBuilder<Workflow> builder)
        {
            builder.ToTable(nameof(Workflow));
            builder.Property(b => b.WorkflowId).HasColumnName("WorkflowID");
        }
    }
}
