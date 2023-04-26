namespace NewsWebSiteScraper.Services.Users
{
    public interface IUserService
    {
        public Task<bool> UserHasViewedTheNewsAsync(int newsId, string userId);
    }
}
