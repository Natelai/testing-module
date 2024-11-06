using Domain.dbo;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Configurations;

public class TagConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {

        builder.HasKey(x => x.Id);

        builder.HasMany(x => x.TestsTags)
            .WithOne(x => x.Tag)
            .HasForeignKey(x => x.TagId);
    }
}