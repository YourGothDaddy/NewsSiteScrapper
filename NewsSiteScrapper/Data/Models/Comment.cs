namespace NewsWebSiteScraper.Data.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime Date { get; set; }
        public int NewsId { get; set; }
        public News News { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
    }

}
