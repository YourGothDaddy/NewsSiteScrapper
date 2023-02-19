namespace NewsWebSiteScraper.Services.News
{
    using OpenQA.Selenium;
    using OpenQA.Selenium.Chrome;
    using OpenQA.Selenium.Interactions;
    using OpenQA.Selenium.Support.UI;
    using System.Collections.ObjectModel;
    using System.Text.RegularExpressions;
    using System.Xml.Linq;

    public class DunavmostBackgroundScraperService : IHostedService, IDisposable
    {
        private Timer _timer;

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(Scrape, null, TimeSpan.Zero,
                TimeSpan.FromMinutes(30)); // run every 5 minutes

            return Task.CompletedTask;
        }

        private void Scrape(object state)
        {
            using (var driver = new ChromeDriver())
            {
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
                wait.PollingInterval = TimeSpan.FromMilliseconds(2000);

                driver.Navigate().GoToUrl("https://www.dunavmost.com/Ruse");

                var cookies = driver.FindElement(By.XPath("/html/body/div[4]/div[2]/div[1]/div[2]/div[2]/button[1]"));
                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(cookies));
                cookies.Click();

                var lastPage = driver.FindElement(By.XPath("//li/a[text()='Край']"));

                lastPage.Click();

                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector("div.search-result-item")));

                while (true)
                {
                    var pagesUlElement = driver.FindElement(By.XPath("//div[@class='paginate']/ul"));

                    var currentPageElementBeforeRefresh = pagesUlElement.FindElement(By.XPath("//li[a[@class='active']]"));
                    var currentPageBeforeRefresh = currentPageElementBeforeRefresh.Text;

                    try
                    {
                        wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.TagName("ul")));
                    }
                    catch (Exception)
                    {
                        driver.Navigate().Refresh();
                    }

                    pagesUlElement = driver.FindElement(By.XPath("//div[@class='paginate']/ul"));

                    var currentPageElementAfterRefresh = pagesUlElement.FindElement(By.XPath("//li[a[@class='active']]"));
                    var currentPageAfterRefresh = currentPageElementAfterRefresh.Text;

                    while (currentPageAfterRefresh != currentPageBeforeRefresh)
                    {
                        driver.Navigate().Back();
                    }

                    var allUnorderedListsOfNewsElement = driver.FindElement(By.CssSelector("div.search-result-item"));
                    var allUnorderedListsOfNews = allUnorderedListsOfNewsElement.FindElements(By.TagName("ul"));

                    var totalUlCount = allUnorderedListsOfNews.Count;

                    for (int i = totalUlCount; i >= 1; i--)
                    {
                        var ul = allUnorderedListsOfNews[i - 1];

                        var liElementsInUl = ul.FindElements(By.TagName("li"));

                        GoThroughEachNews(driver, liElementsInUl.Count, i, wait, totalUlCount);

                        allUnorderedListsOfNewsElement = driver.FindElement(By.CssSelector("div.search-result-item"));
                        allUnorderedListsOfNews = allUnorderedListsOfNewsElement.FindElements(By.TagName("ul"));
                    }

                    while (true)
                    {
                        try
                        {
                            var previousPageElement = driver.FindElement(By.XPath("//li/a[@alt='Предишна']"));
                            previousPageElement.Click();

                            break;
                        }
                        catch (Exception)
                        {

                            driver.Navigate().Refresh();
                        }
                    }
                }
            }
        }

        private void GoThroughEachNews(ChromeDriver driver, int listOfNewsCount, int ulNumber, WebDriverWait wait, int totalUlCount)
        {
            IWebElement newsElement = null;

            for (int i = listOfNewsCount; i >= 1; i--)
            {
                while (true)
                {
                    try
                    {
                        wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.XPath($"/html/body/div[1]/section[1]/main/div[3]/ul[{ulNumber}]/li[{i}]")));

                        break;
                    }
                    catch (Exception)
                    {
                        driver.Navigate().Refresh();
                    }
                }

                if (totalUlCount == 1)
                {
                    newsElement = driver.FindElement(By.XPath($"/html/body/div[1]/section[1]/main/div[3]/ul/li[{i}]"));
                }
                else
                {
                    newsElement = driver.FindElement(By.XPath($"/html/body/div[1]/section[1]/main/div[3]/ul[{ulNumber}]/li[{i}]"));
                }

                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(false);", newsElement);
                newsElement.Click();

                var insElements = driver.FindElements(By.CssSelector("ins.adsbygoogle.adsbygoogle-noablate"));

                if (insElements.Count > 0)
                {
                    driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(15);
                    try
                    {
                        ClickAnywhereOnThePage(driver);
                    }
                    catch (Exception)
                    {
                        driver.Navigate().Refresh();
                        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(15);
                        ClickAnywhereOnThePage(driver);
                    }
                    driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(15);
                }

                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(15);

                IWebElement titleElement = null;
                string title = null;

                try
                {
                    titleElement = driver.FindElement(By.XPath($"/html/body/div[1]/section[1]/main/article/header/h1"));
                    title = titleElement.Text;
                }
                catch (Exception)
                {

                    driver.Navigate().Refresh();

                    titleElement = driver.FindElement(By.XPath($"/html/body/div[1]/section[1]/main/article/header/h1"));
                    title = titleElement.Text;
                }

                IWebElement imageElement = null;
                try
                {
                    imageElement = driver.FindElement(By.XPath("/html/body/div[1]/section[1]/main/article/figure/div/img"));
                }
                catch (NoSuchElementException)
                {
                    imageElement = null;
                }

                if (imageElement != null)
                {
                    var image = imageElement.GetAttribute("src");
                }

                var contentElement = driver.FindElement(By.ClassName("news_text"));
                var contentParagraphs = contentElement.FindElements(By.TagName("p"));

                var paragraphs = new List<string>();

                foreach (var paragraph in contentParagraphs)
                {
                    var paragraphInnerHtml = paragraph.GetAttribute("innerHTML");
                    var sanitized = RemoveHtmlTags(paragraphInnerHtml);
                    paragraphs.Add(sanitized);
                }

                var currentNews = new Data.Models.News();

                driver.Navigate().Back();

                var random = new Random();

                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(random.Next(15, 60));

                bool PaginateDivExists = driver.FindElements(By.CssSelector("div.paginate")).Count > 0;
                if (!PaginateDivExists)
                {
                    driver.Navigate().Back();
                    driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(random.Next(15, 60));
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        public string RemoveHtmlTags(string html)
        {
            // Use a regular expression to remove all tags except for <strong> with no attributes
            string pattern = @"<(?!strong(?:\s*(?!\s*\b(id|class)\b)[^>]*)?>|/strong(?:\s*(?!\s*\b(id|class)\b)[^>]*)?>)([^>]*)>|</(?!strong(?:\s*(?!\s*\b(id|class)\b)[^>]*)?>|/strong(?:\s*(?!\s*\b(id|class)\b)[^>]*)?>)[^>]*>";

            string strippedHtml = Regex.Replace(html, pattern, "");

            return strippedHtml;
        }

        public static void ClickAnywhereOnThePage(ChromeDriver driver)
        {
            // Get the dimensions of the page
            var pageWidth = driver.Manage().Window.Size.Width;
            var pageHeight = driver.Manage().Window.Size.Height;

            // Set the x coordinate to the center of the page
            var x = 25;

            // Set the y coordinate to 0 (top of the page)
            var y = 25;

            // Create a new Actions object
            var builder = new Actions(driver);

            // Move the mouse to the random position
            builder.MoveToElement(driver.FindElement(By.TagName("body")), x, y).Click().Perform();
        }
    }
}
