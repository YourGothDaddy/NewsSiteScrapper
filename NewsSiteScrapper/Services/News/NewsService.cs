namespace NewsWebSiteScraper.Services.News
{
    using Microsoft.EntityFrameworkCore;
    using NewsWebSiteScraper.Data;
    using NewsWebSiteScraper.Data.Models;
    using NewsWebSiteScraper.Models.News;

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

        public async Task<List<DisplayListOfNewsViewModel>> RetrieveAllNewsAsync()
        {
            var allNews = await this.data
                .News
                .Select(n => new DisplayListOfNewsViewModel
                {
                    Title = n.Title,
                    ImageUrl = n.ImageUrl
                })
                .Take(10)
                .ToListAsync();

            return allNews;
        }
    }
}
