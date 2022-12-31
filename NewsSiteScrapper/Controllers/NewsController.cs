namespace NewsSiteScrapper.Controllers
{
    using HtmlAgilityPack;
    using Microsoft.AspNetCore.Mvc;
    using NewsSiteScrapper.Models.News;
    using System.Text.RegularExpressions;

    public class NewsController : Controller
    {
        public async Task<IActionResult> Dunavmost()
        {
            // Send an HTTP request to the page
            var client = new HttpClient();
            var response = client.GetAsync("https://www.dunavmost.com/Ruse/News").Result;

            // Load the HTML content of the page into an HtmlDocument
            var doc = new HtmlDocument();
            doc.Load(response.Content.ReadAsStreamAsync().Result);

            // Extract the title and body of the first article on the page
            var hrefNode = doc.DocumentNode.SelectSingleNode("//*[@id=\"left_column\"]/main/div[3]/ul[1]/li[1]/article/a");
            var href = hrefNode.GetAttributeValue("href", "");

            var page = "https://www.dunavmost.comChange".Replace("Change", href);

            response = client.GetAsync(page).Result;

            doc.Load(response.Content.ReadAsStreamAsync().Result);

            var titleNode = doc.DocumentNode.SelectSingleNode("//*[@id=\"news\"]/header/h1");
            var title = titleNode.InnerHtml;

            var imageNode = doc.DocumentNode.SelectSingleNode("//*[@id=\"news\"]/figure/div[1]/img");
            var image = imageNode.GetAttributeValue("src", "");

            var imageUrl = "https://www.dunavmost.comChange".Replace("Change", image);

            var contentNode = doc.DocumentNode.SelectSingleNode("//*[@id=\"news\"]/div[3]");
            doc.LoadHtml(contentNode.InnerHtml);

            var paragraphs = new List<string>();

            foreach (var item in doc.DocumentNode.SelectNodes("//p"))
            {
                var sanitized = Regex.Replace(item.InnerHtml, "<[^>]*>", "");

                paragraphs.Add(sanitized);
            }

            var model = new DisplayNewsViewModel();
            model.Title = title;
            model.Content = paragraphs;
            model.ImageSrc = imageUrl;

            return View(model);
        }
    }
}
