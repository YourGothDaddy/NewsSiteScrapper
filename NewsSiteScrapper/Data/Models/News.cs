namespace NewsWebSiteScraper.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    public class News
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string ImageUrl { get; set; }
        [Required]
        public string Content { get; set; } = string.Empty;

        public DateTime Date { get; set; }

        public int UniqueViews { get; set; }
    }
}
