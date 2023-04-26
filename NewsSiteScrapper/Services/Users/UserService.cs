namespace NewsWebSiteScraper.Services.Users
{
    using Microsoft.EntityFrameworkCore;
    using NewsWebSiteScraper.Data;
    using NewsWebSiteScraper.Data.Models;

    public class UserService : IUserService
    {
        private readonly NewsWebSiteScraperDbContext data;
        public UserService(NewsWebSiteScraperDbContext data)
        {
            this.data = data;
        }
        public async Task<bool> UserHasViewedTheNewsAsync(int newsId, string userId)
        {
            var hasViewed = await this.data
                .NewsViews
                .AnyAsync(nv => nv.NewsId == newsId && nv.UserId == userId);

            return hasViewed;
        }
    }
}
