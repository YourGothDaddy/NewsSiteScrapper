namespace NewsWebSiteScraper.Models.News
{
    public class NewsDetailsViewModel
    {
        public NewsModel News { get; set; }

        public List<CommentModel> Comments { get; set; }
    }
}
