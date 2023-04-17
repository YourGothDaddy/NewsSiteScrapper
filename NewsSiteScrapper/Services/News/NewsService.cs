namespace NewsWebSiteScraper.Services.News
{
    using NewsWebSiteScraper.Data;
    using NewsWebSiteScraper.Data.Models;
    public class NewsService : INewsService
    {
        private readonly NewsWebSiteScraperDbContext data;

        public NewsService(NewsWebSiteScraperDbContext data)
        {
            this.data = data;
        }

        public void SaveNews(string title, string imageUrl, string content)
        {
            var news = new News
            {
                Title = title,
                ImageUrl = imageUrl,
                Content = content
            };

            data.News.Add(news);
        }
    }
}
