using IceSync.Infrastructure.Domain;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

#pragma warning disable CS1591
#pragma warning disable SA1600

namespace IceSync.Data.Configuration
{
    public class TokenConfiguration : IEntityTypeConfiguration<Token>
    {
        public void Configure(EntityTypeBuilder<Token> builder)
        {
            builder.ToTable(nameof(Token));
            builder.HasKey(b => b.Value);
        }
    }
}
