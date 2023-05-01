namespace NewsWebSiteScraper.Models.News
{
    public class CommentModel
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime Date { get; set; }
        public int NewsId { get; set; }
        public string UserId { get; set; }
        public string FullName { get; set; }
    }

}
