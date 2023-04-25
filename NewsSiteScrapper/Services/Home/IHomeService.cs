namespace NewsWebSiteScraper.Services.Home
{
    using NewsWebSiteScraper.Models.News;

    public interface IHomeService
    {
        public Task<List<NewsModel>> RetrieveMostRecentNewsAsync(int numberOfNewsToRetrieve);
    }
}
