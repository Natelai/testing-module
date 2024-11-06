using Domain.dbo;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class FavouriteTestsConfiguration : IEntityTypeConfiguration<FavouriteTests>
{
    public void Configure(EntityTypeBuilder<FavouriteTests> builder)
    {

        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.User)
            .WithMany(x => x.FavouriteTests)
            .HasForeignKey(x => x.UserId);

        builder.HasOne(x => x.Test)
            .WithMany(x => x.FavouriteTests)
            .HasForeignKey(x => x.TestId);
    }
}