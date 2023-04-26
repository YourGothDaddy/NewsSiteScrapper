namespace NewsWebSiteScraper.Data
{
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using NewsWebSiteScraper.Data.Models;
    public class NewsWebSiteScraperDbContext : IdentityDbContext<User>
    {
        public NewsWebSiteScraperDbContext(DbContextOptions<NewsWebSiteScraperDbContext> options)
            : base(options)
        {
        }

        public DbSet<News> News { get; set; }

        public DbSet<NewsViews> NewsViews { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<News>(entity =>
            {
                entity.HasKey(x => x.Id);
            });

            builder.Entity<NewsViews>()
                .HasKey(nv => new { nv.NewsId, nv.UserId });

            base.OnModelCreating(builder);
        }
    }
}