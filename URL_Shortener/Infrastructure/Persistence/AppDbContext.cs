using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using URL_Shortener.Domain.Entities;

namespace URL_Shortener.Infrastructure.Persistence;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public DbSet<ShortUrl> ShortUrls => Set<ShortUrl>();
    public DbSet<AboutInfo> AboutInfos => Set<AboutInfo>();

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ShortUrl>(entity =>
        {
            entity.HasIndex(x => x.OriginalUrl).IsUnique();
            entity.HasIndex(x => x.ShortCode).IsUnique();

            entity.Property(x => x.OriginalUrl).IsRequired();
            entity.Property(x => x.ShortCode).IsRequired();
            entity.Property(x => x.CreatedAtUtc).IsRequired();

            entity.HasOne(x => x.CreatedByUser)
                .WithMany()
                .HasForeignKey(x => x.CreatedByUserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<AboutInfo>(entity =>
        {
            entity.Property(x => x.Content).IsRequired();
        });
    }
}