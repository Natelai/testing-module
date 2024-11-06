using Domain.dbo;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {

        builder.Property(x => x.IsPremium)
            .HasDefaultValue(false);

        builder.HasMany(x => x.CompletedTests)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId);

        builder.HasMany(x => x.FavouriteTests)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId);
    }
}