namespace NewsWebsiteSiteScraper.Controllers
{
    using System.Text.RegularExpressions;
    using Microsoft.AspNetCore.Mvc;
    using NewsWebSiteScraper.Models.News;
    using NewsWebSiteScraper.Services.News;

    public class NewsController : Controller
    {
        private readonly INewsService news;

        public NewsController(INewsService news)
        {
            this.news = news;
        }
        public async Task<IActionResult> Bulgaria()
        {
            var allNews = await this.news.RetrieveAllNewsAsync();

            return View(allNews);
        }
        public string RemoveHtmlTags(string html)
        {
            // Use a regular expression to remove all tags except for <strong> with no attributes
            string pattern = @"<(?!strong(?:\s*(?!\s*\b(id|class)\b)[^>]*)?>|/strong(?:\s*(?!\s*\b(id|class)\b)[^>]*)?>)([^>]*)>|</(?!strong(?:\s*(?!\s*\b(id|class)\b)[^>]*)?>|/strong(?:\s*(?!\s*\b(id|class)\b)[^>]*)?>)[^>]*>";
            string strippedHtml = Regex.Replace(html, pattern, "");
            return strippedHtml;
        }
    }
}
