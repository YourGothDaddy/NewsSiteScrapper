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

        public DbSet<News> News { get; set; }
    }
}