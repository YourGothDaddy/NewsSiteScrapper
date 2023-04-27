namespace NewsWebSiteScraper.Services.Home
{
    using Microsoft.EntityFrameworkCore;
    using NewsWebSiteScraper.Data;
    using NewsWebSiteScraper.Models.Home;
    using NewsWebSiteScraper.Models.News;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class HomeService : IHomeService
    {
        private readonly NewsWebSiteScraperDbContext data;

        public HomeService(NewsWebSiteScraperDbContext data)
        {
            this.data = data;
        }
        public async Task<List<NewsModel>> RetrieveMostRecentNewsAsync(int numberOfNewsToRetrieve)
        {
            var recentNews = await this.data
                .News
                .OrderByDescending(n => n.Date)
                .Take(this.data.News.Count() < numberOfNewsToRetrieve ? this.data.News.Count() : numberOfNewsToRetrieve)
                .Select(n => new NewsModel
                {
                    Id = n.Id,
                    Title = n.Title,
                    ImageUrl = n.ImageUrl,
                    Date = n.Date
                })
                .ToListAsync();

            return recentNews;
        }

        public async Task<List<NewsModel>> RetrieveMostViewedNewsAsync(int numberOfNewsToRetrieve)
        {
            var mostViewedNews = await this.data
                .News
                .OrderByDescending(n => n.UniqueViews)
                .Take(this.data.News.Count() < numberOfNewsToRetrieve ? this.data.News.Count() : numberOfNewsToRetrieve)
                .Select(n => new NewsModel
                {
                    Id = n.Id,
                    Title = n.Title,
                    ImageUrl = n.ImageUrl,
                    Date = n.Date
                })
                .ToListAsync();

            return mostViewedNews;
        }
    }
}
