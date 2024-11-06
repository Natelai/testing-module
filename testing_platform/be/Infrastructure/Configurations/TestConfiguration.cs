using Domain.dbo;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class TestConfiguration : IEntityTypeConfiguration<Test>
{
    public void Configure(EntityTypeBuilder<Test> builder)
    {

        builder.HasKey(x => x.Id);

        builder.HasMany(x => x.TestTags)
            .WithOne(x => x.Test)
            .HasForeignKey(x => x.TestId);

        builder.HasMany(x => x.FavouriteTests)
            .WithOne(x => x.Test)
            .HasForeignKey(x => x.TestId);

        builder.HasMany(x => x.CompletedTests)
            .WithOne(x => x.Test)
            .HasForeignKey(x => x.TestId);
    }
}