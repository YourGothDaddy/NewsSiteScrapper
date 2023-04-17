namespace NewsWebsiteSiteScraper.Controllers
{
    using System.Text.RegularExpressions;
    using HtmlAgilityPack;
    using Microsoft.AspNetCore.Mvc;
    using NewsWebSiteScraper.Models.News;
    public class NewsController : Controller
    {
        public async Task<IActionResult> Dunavmost()
        {
            // Send an HTTP request to the page
            var client = new HttpClient();
            var response = await client.GetAsync("https://www.dunavmost.com/Ruse/News");
            // Load the HTML content of the page into an HtmlDocument
            var doc = new HtmlDocument();
            doc.Load(await response.Content.ReadAsStreamAsync());
            // Extract the title and body of the first article on the page
            var hrefNode = doc.DocumentNode.SelectSingleNode("//*[@id=\"left_column\"]/main/div[3]/ul[1]/li[1]/article/a");
            var href = hrefNode.GetAttributeValue("href", "");
            // Create the full URL of the article page by replacing "Change" with the URL of the article
            var page = "https://www.dunavmost.comChange".Replace("Change", href);
            // Send an HTTP request to the article page
            response = await client.GetAsync(page);
            doc.Load(await response.Content.ReadAsStreamAsync());
            // Extract the title, image, and content of the article
            var titleNode = doc.DocumentNode.SelectSingleNode("//*[@id=\"news\"]/header/h1");
            var title = titleNode.InnerHtml;
            var imageNode = doc.DocumentNode.SelectSingleNode("//*[@id=\"news\"]/figure/div[1]/img");
            var image = imageNode.GetAttributeValue("src", "");
            var imageUrl = "https://www.dunavmost.comChange".Replace("Change", image);
            string iFrame = null;
            var iFrameNode = doc.DocumentNode.SelectSingleNode("/html/body/div[1]/section[1]/main/article/div[3]/iframe");
            if (iFrameNode != null)
            {
                iFrame = iFrameNode.OuterHtml;
            }
            var contentNode = doc.DocumentNode.SelectSingleNode("//*[@id=\"news\"]/div[3]");
            doc.LoadHtml(contentNode.InnerHtml);
            // Extract the paragraphs of the content and sanitize the HTML tags
            var paragraphs = new List<string>();
            foreach (var item in doc.DocumentNode.SelectNodes("//p"))
            {
                var sanitized = RemoveHtmlTags(item.InnerHtml);
                paragraphs.Add(sanitized);
            }
            // Create a DisplayNewsViewModel object with the extracted data and pass it to the view
            var model = new DisplayNewsViewModel();
            model.Title = title;
            model.Content = paragraphs;
            model.ImageSrc = imageUrl;
            model.iFrame = iFrame;
            return View(model);
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
