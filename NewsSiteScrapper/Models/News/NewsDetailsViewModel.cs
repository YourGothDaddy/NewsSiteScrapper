namespace NewsWebSiteScraper.Models.News
{
    public class NewsDetailsViewModel
    {
        public NewsModel News { get; set; }

        public List<CommentModel> Comments { get; set; }

        public int? PageNumber { get; set; }

        public int NumberOfCommentsOnPage { get; set; }

        public int TotalCommentsCount { get; set; }

        public int TotalPages { get; set; }
    }
}
