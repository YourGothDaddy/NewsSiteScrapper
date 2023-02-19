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
        public List<string> content { get; set; } = new List<string>();
    }
}
