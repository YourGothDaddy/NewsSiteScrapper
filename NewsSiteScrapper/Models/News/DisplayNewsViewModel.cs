namespace NewsWebSiteScraper.Models.News
{
    using System.ComponentModel.DataAnnotations;
    public class DisplayNewsViewModel
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public List<string> Content { get; set; }
        [Required]
        public string ImageSrc { get; set; }
        public string? iFrame { get; set; }
    }
}
