namespace NewsWebSiteScraper.Models.News
{
    public class DisplayNewsViewModel
    {
        public string? Title { get; set; }
        public string? ImageUrl { get; set; }
        public string? Content { get; set; }

        public DateTime? Date { get; set; }
    }
}
