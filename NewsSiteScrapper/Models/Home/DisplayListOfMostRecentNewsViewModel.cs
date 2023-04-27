namespace NewsWebSiteScraper.Models.Home
{
    using NewsWebSiteScraper.Models.News;

    public class DisplayListOfMostRecentNewsViewModel
    {
        public List<NewsModel> News { get; set; }

        public List<NewsModel> MostViewedNews { get; set; }
    }
}
