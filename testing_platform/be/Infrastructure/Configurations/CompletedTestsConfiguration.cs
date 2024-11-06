using Domain.dbo;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class CompletedTestsConfiguration : IEntityTypeConfiguration<CompletedTests>
{
    public void Configure(EntityTypeBuilder<CompletedTests> builder)
    {

        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.User)
            .WithMany(x => x.CompletedTests)
            .HasForeignKey(x => x.UserId);

        builder.HasOne(x => x.Test)
            .WithMany(x => x.CompletedTests)
            .HasForeignKey(x => x.TestId);
    }
}