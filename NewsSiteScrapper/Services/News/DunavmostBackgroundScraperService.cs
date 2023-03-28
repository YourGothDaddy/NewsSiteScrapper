namespace NewsWebSiteScraper.Services.News
{
    using OpenQA.Selenium;
    using OpenQA.Selenium.Chrome;
    using OpenQA.Selenium.Support.UI;
    using SeleniumExtras.WaitHelpers;
    using Serilog;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    public class DunavmostBackgroundScraperService : IHostedService, IDisposable
    {
        private Timer _timer;

        public void Dispose()
        {
            _timer?.Dispose();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(Scrape, null, TimeSpan.Zero,
                TimeSpan.FromMinutes(15000)); // run every 120 minutes
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        private void Scrape(object state)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File(
                    $"logs/log-{DateTime.Now:MM-dd-yyyy-hh-mm-tt}.txt",
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();

            var timer = new Stopwatch();

            var service = ChromeDriverService.CreateDefaultService();
            service.LogPath = @"logs/chromedriver.log";

            var options = new ChromeOptions();
            //options.AddArgument("--headless");
            options.AddArgument("--disable-gpu");
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-logging");
            options.AddArgument("--log-level=3");
            options.SetLoggingPreference("browser", LogLevel.Off);
            options.SetLoggingPreference("client", LogLevel.Off);
            options.SetLoggingPreference("driver", LogLevel.Off);
            options.SetLoggingPreference("performance", LogLevel.Off);
            options.SetLoggingPreference("server", LogLevel.Off);
            options.SetLoggingPreference("selenium", LogLevel.Off);

            using (var driver = new ChromeDriver(service, options, TimeSpan.FromSeconds(600)))
            {
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(600));

                //Go to the website
                driver.Navigate().GoToUrl("https://www.dnes.bg/news.php?last&cat=1&page=1");

                //Click cookies button

                wait.Until(ExpectedConditions.ElementExists(By.CssSelector("button.fc-button.fc-cta-consent")));

                var cookiesButton = driver.FindElement(By.CssSelector("button.fc-button.fc-cta-consent"));
                wait.Until(ExpectedConditions.ElementToBeClickable(cookiesButton));
                cookiesButton.Click();

                //Get the div holding different news
                var allNewsDiv = driver.FindElement(By.CssSelector("div#c1"));

                //Get all the news elements
                var allNews = allNewsDiv.FindElements(By.CssSelector("div.b2"));

                var newsCount = allNews.Count();

                //Current URL and page before browsing the news
                var currentUrl = driver.Url;
                var currentPage = int.Parse(currentUrl.Substring(currentUrl.Length - 1, 1));
                while (true)
                {
                    GoThroughEachNews(driver, wait, allNewsDiv, allNews, newsCount, currentUrl, timer);

                    currentPage++;
                    currentUrl = $"https://www.dnes.bg/news.php?last&cat=1&page={currentPage}";
                    driver.Navigate().GoToUrl(currentUrl);
                }
            }
        }

        private static async void GoThroughEachNews(ChromeDriver driver, WebDriverWait wait, IWebElement allNewsDiv, ReadOnlyCollection<IWebElement> allNews, int newsCount, string currentUrl, Stopwatch timer)
        {
            for (int i = 0; i < newsCount - 1; i++)
            {
                //Get the div holding different news
                allNewsDiv = driver.FindElement(By.CssSelector("div#c1"));

                //Get all the news elements
                allNews = allNewsDiv.FindElements(By.CssSelector("div.b2"));

                var currentNews = allNews[i];
                wait.Until(ExpectedConditions.ElementExists(By.CssSelector("div.ttl")));
                var currentNewsTitle = currentNews.FindElement(By.CssSelector("div.ttl"));

                var aElement = currentNewsTitle.FindElement(By.TagName("a"));

                try
                {
                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(false);", aElement);
                    aElement.Click();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    driver.Navigate().GoToUrl(currentUrl);
                    continue;
                }

                var uri = new Uri(driver.Url);
                var title = uri.Segments[uri.Segments.Length - 1];

                Console.WriteLine($"SAVE THE DATA FOR {title}");

                driver.Navigate().GoToUrl(currentUrl);

                timer.Start();

                while (true)
                {
                    if (timer.Elapsed.TotalSeconds > 1)
                    {
                        timer.Stop();

                        Console.WriteLine($"Time taken: {timer.Elapsed.TotalSeconds}");

                        timer.Reset();

                        break;
                    }
                }

            }
        }
    }
}
