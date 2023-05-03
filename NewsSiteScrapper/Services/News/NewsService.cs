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
                    Id = n.Id,
                    Title = n.Title,
                    ImageUrl = n.ImageUrl,
                    Content = n.Content,
                    Date = n.Date,
                    UniqueViews = n.UniqueViews
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

        public async Task IncrementUniqueNewsAsync(int id, string userId)
        {
            var newsItem = await this.data
                .News
                .Where(n => n.Id == id)
                .FirstOrDefaultAsync();

            newsItem.UniqueViews += 1;

            await this.data
                .NewsViews
                .AddAsync(new NewsViews { NewsId = id, UserId = userId });

            await this.data.SaveChangesAsync();
        }

        public async Task AddCommentAsync(Comment comment)
        {
            await this.data.Comments.AddAsync(comment);
            await this.data.SaveChangesAsync();
        }

        public async Task<List<CommentModel>> RetrieveAllCommentsForThePageAsync(int newsId, int? pageNumber, int numberOfCommentsOnPage)
        {
            var allComments = await this.data
                .Comments
                .Include(x => x.User)
                .Where(c => c.NewsId == newsId)
                .OrderByDescending(c => c.Date)
                .Select(c => new CommentModel
                {
                    FullName = c.User.FullName,
                    Content = c.Content
                })
                .Skip((pageNumber.Value - 1) * numberOfCommentsOnPage)
                .Take(numberOfCommentsOnPage)
                .ToListAsync();

            return allComments;
        }

        public async Task<int> RetrieveAllCommentsCountAsync(int newsId)
        {
            var totalCommentsCount = await this.data
                .Comments
                .Where(c => c.NewsId == newsId)
                .CountAsync();

            return totalCommentsCount;
        }
    }
}
