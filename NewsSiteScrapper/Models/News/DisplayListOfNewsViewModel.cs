namespace NewsWebSiteScraper.Models.News
{
    public class DisplayListOfNewsViewModel
    {
        public List<NewsModel> News { get; set; }
        public int? PageNumber { get; set; }

        public int NumberOfNewsOnPage { get; set; }

        public int TotalNewsCount { get; set; }

        public int TotalPages { get; set; }
    }
}
