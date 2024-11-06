using Domain.dbo;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class TestTagsConfiguration : IEntityTypeConfiguration<TestTags>
{
    public void Configure(EntityTypeBuilder<TestTags> builder)
    {

        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.Tag)
            .WithMany(x => x.TestsTags)
            .HasForeignKey(x => x.TagId);

        builder.HasOne(x => x.Test)
            .WithMany(x => x.TestTags)
            .HasForeignKey(x => x.TestId);
    }
}