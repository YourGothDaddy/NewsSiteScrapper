namespace NewsWebSiteScraper.Services.News
{
    using NewsWebSiteScraper.Models.News;

    public interface INewsService
    {
        public void SaveNews(string title,
            string imageUrl,
            string content);

        public Task<List<DisplayListOfNewsViewModel>> RetrieveAllNewsAsync();
    }
}
