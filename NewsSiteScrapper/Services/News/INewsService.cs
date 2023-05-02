namespace NewsWebSiteScraper.Services.News
{
    using NewsWebSiteScraper.Data.Models;
    using NewsWebSiteScraper.Models.News;

    public interface INewsService
    {
        public void SaveNews(string title,
            string imageUrl,
            string content);

        public Task<List<NewsModel>> RetrieveAllNewsForThePageAsync(int? pageNumber,
            int numberOfNewsOnPage);

        public Task<NewsModel> RetrieveNewsAsync(int id);

        public Task<int> RetrieveAllNewsCountAsync();

        public Task IncrementUniqueNewsAsync(int id, string userId);

        public Task AddCommentAsync(Comment comment);

        public Task<List<CommentModel>> RetrieveAllCommentsForThePageAsync(int newsId, int? pageNumber, int numberOfCommentsOnPage);

        public Task<int> RetrieveAllCommentsCountAsync();
    }
}
