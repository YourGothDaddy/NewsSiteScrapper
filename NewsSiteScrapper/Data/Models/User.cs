namespace NewsWebSiteScraper.Data.Models
{
    using Microsoft.AspNetCore.Identity;

    public class User : IdentityUser
    {
        public string FullName { get; set; }
        public string ImageUrl { get; set; }
    }
}
