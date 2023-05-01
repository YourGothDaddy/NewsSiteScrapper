namespace NewsWebSiteScraper.Data
{
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using NewsWebSiteScraper.Data.Models;
    using System.Reflection.Emit;

    public class NewsWebSiteScraperDbContext : IdentityDbContext<User>
    {
        public NewsWebSiteScraperDbContext(DbContextOptions<NewsWebSiteScraperDbContext> options)
            : base(options)
        {
        }

        public DbSet<News> News { get; set; }

        public DbSet<NewsViews> NewsViews { get; set; }

        public DbSet<Comment> Comments { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<News>(entity =>
            {
                entity.HasKey(x => x.Id);
            });

            builder.Entity<NewsViews>()
                .HasKey(nv => new { nv.NewsId, nv.UserId });

            builder.Entity<Comment>()
                .HasKey(c => c.Id);

            builder.Entity<Comment>()
                .HasOne(c => c.News)
                .WithMany(n => n.Comments)
                .HasForeignKey(c => c.NewsId);

            builder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.UserId);

            base.OnModelCreating(builder);
        }
    }
}