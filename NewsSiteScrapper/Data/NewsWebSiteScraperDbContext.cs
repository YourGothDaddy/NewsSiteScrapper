namespace NewsWebSiteScraper.Data
{
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using NewsWebSiteScraper.Data.Models;
    public class NewsWebSiteScraperDbContext : IdentityDbContext
    {
        public NewsWebSiteScraperDbContext(DbContextOptions<NewsWebSiteScraperDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<News>(entity =>
            {
                entity.HasKey(x => x.Id);
            });

            base.OnModelCreating(builder);
        }
        public DbSet<News> News { get; set; }
    }
}