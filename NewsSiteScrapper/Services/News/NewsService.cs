namespace NewsWebSiteScraper.Services.News
{
    using NewsWebSiteScraper.Data;
    using NewsWebSiteScraper.Services.News;

    public class NewsService : INewsService
    {
        private readonly NewsWebSiteScraperDbContext data;

        public void SaveNews(string title, string imageUrl, string content)
        {
            throw new NotImplementedException();
        }
    }
}
