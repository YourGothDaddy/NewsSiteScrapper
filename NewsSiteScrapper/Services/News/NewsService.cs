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

        public async Task<List<NewsModel>> RetrieveAllNewsForThePageAsync(int? pageNumber, int numberOfNewsOnPage)
        {
            var allNews = await this.data
                .News
                .OrderByDescending(n => n.Date)
                .Where(n => n.Date != new DateTime(1000, 1, 1, 12, 30, 0))
                .Select(n => new NewsModel
                {
                    Id = n.Id,
                    Title = n.Title,
                    ImageUrl = n.ImageUrl
                })
                .Skip((pageNumber.Value - 1) * numberOfNewsOnPage)
                .Take(numberOfNewsOnPage)
                .ToListAsync();

            return allNews;
        }

        public async Task<NewsModel> RetrieveNewsAsync(int id)
        {
            var news = await this.data
                .News
                .Where(n => n.Id == id)
                .Select(n => new NewsModel
                {
                    Title = n.Title,
                    ImageUrl = n.ImageUrl,
                    Content = n.Content,
                    Date = n.Date
                })
                .FirstOrDefaultAsync();

            return news;
        }

        public async Task<int> RetrieveAllNewsCountAsync()
        {
            var totalNewsCount = await this.data
                .News
                .Where(n => n.Date != new DateTime(1000, 1, 1, 12, 30, 0))
                .CountAsync();

            return totalNewsCount;

        }
    }
}
