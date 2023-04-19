namespace NewsWebSiteScraper.Services.News
{
    using NewsWebSiteScraper.Models.News;

    public interface INewsService
    {
        public void SaveNews(string title,
            string imageUrl,
            string content);

        public Task<List<NewsModel>> RetrieveAllNewsForThePageAsync(int? pageNumber,
            int numberOfNewsOnPage);

        public Task<int> RetrieveAllNewsCountAsync();
    }
}
