using Domain.dbo;
using Infrastructure.Configurations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public class AppDbContext : IdentityDbContext<User, IdentityRole<int>, int>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<CompletedTests> CompletedTests { get; set; }
    public DbSet<FavouriteTests> FavouriteTests { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<Test> Tests { get; set; }
    public DbSet<TestTags> TestTags { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfiguration(new CompletedTestsConfiguration());
        builder.ApplyConfiguration(new FavouriteTestsConfiguration());
        builder.ApplyConfiguration(new TagConfiguration());
        builder.ApplyConfiguration(new TestConfiguration());
        builder.ApplyConfiguration(new TestTagsConfiguration());
        builder.ApplyConfiguration(new UserConfiguration());
    }
}