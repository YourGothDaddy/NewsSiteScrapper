﻿namespace NewsWebSiteScraper.Services.News
{
    using HtmlAgilityPack;
    using Microsoft.Data.SqlClient;
    using Microsoft.EntityFrameworkCore;
    using NewsWebSiteScraper.Constants;
    using NewsWebSiteScraper.Data;
    using NewsWebSiteScraper.Data.Models;
    using System.Diagnostics;
    using System.Globalization;
    using System.Text;
    using System.Text.RegularExpressions;
 
    public class BackgroundScraperService : IHostedService, IDisposable
    {
        private Timer _timer;
        private readonly IDbContextFactory<NewsWebSiteScraperDbContext> data;

        public BackgroundScraperService(IDbContextFactory<NewsWebSiteScraperDbContext> data)
        {
            this.data = data;
        }
 
        private async void Scrape(object state)
        {
            Console.OutputEncoding = Encoding.UTF8;
            var fileName = ServiceConstants.LastProcessedPageTxtName;
            var lastPage = 1;

            var web = new HtmlWeb();
            var doc = new HtmlDocument();

            var pageToGo = "";

            var links = new List<string>();
            var news = new List<News>();

            var tooManyPagesWithoutScrapedNews = false;
            var pagesWithoutScrapedNews = 0;
            while (true)
            {
                if (File.Exists(fileName))
                {
                    var lastProcessedPageStr = File.ReadAllText(fileName);
                    int.TryParse(lastProcessedPageStr, out lastPage);
                    Console.WriteLine($"Last processed page is: {lastPage}");
                }
                else
                {
                    using (File.Create(fileName)) { }
                }

                if (lastPage > 1)
                {
                    doc = NavigateToWebsite(web, lastPage);
                }
                else
                {
                    doc = NavigateToWebsite(web, 1);
                }

                Console.WriteLine("Went to the website");

                // Get the first <a> element with class "pagination-next"
                var nextPageLink = doc.DocumentNode.SelectSingleNode("//a[@class='pagination-next']");

                while (nextPageLink != null)
                {

                    try
                    {
                        var nodes = doc.DocumentNode.SelectNodes("//div[@id='c1']/div[contains(@class, 'b2')]/div[contains(@class, 'ttl')]/a\r\n");

                        if (nodes != null)
                        {
                            await GoThroughEachNewsAsync(links, nodes);
                            Console.WriteLine($"Last page is {lastPage}");

                            if (lastPage % 1 == 0)
                            {
                                var newsData = await GetTheNewsDataAsync(web, links);

                                if (newsData.Count > 0)
                                {
                                    await SaveNewsAsync(newsData);
                                    Console.WriteLine($"Saved {newsData.Count} news!");
                                    newsData.Clear();
                                    links.Clear();
                                    lastPage++;
                                    pagesWithoutScrapedNews = 0;
                                }
                                else
                                {
                                    lastPage++;
                                    pagesWithoutScrapedNews++;
                                }

                                if (pagesWithoutScrapedNews >= 10)
                                {
                                    tooManyPagesWithoutScrapedNews = true;
                                    lastPage = 1;
                                    pagesWithoutScrapedNews = 0;
                                }
                            }

                            File.WriteAllText(fileName, lastPage.ToString());
                            pageToGo = $"https://www.dnes.bg/news.php?last&cat=1&page={lastPage}";
                            doc = NavigateToNextNewsPage(web, pageToGo);
                            nextPageLink = doc.DocumentNode.SelectSingleNode("//a[@class='pagination-next']");
                            await Task.Delay(3600000);
                        }

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }

                File.WriteAllText(fileName, "1");
            }
        }

        private async Task GoThroughEachNewsAsync(List<string> links, HtmlNodeCollection nodes)
        {
            var newsTitle = "";
            foreach (var node in nodes)
            {
                newsTitle = node.InnerHtml;

                if (!await CheckIfNewsExistsAsync(newsTitle))
                {
                    links.Add(node.Attributes["href"].Value);
                }
            }
        }
 
        private HtmlDocument NavigateToWebsite(HtmlWeb web, int page)
        {
            var doc = web.Load($"https://www.dnes.bg/news.php?last&cat=1&page={page}");
            return doc;
        }

        private HtmlDocument NavigateToNextNewsPage(HtmlWeb web, string url)
        {
            var doc = web.Load($"{url}");
            return doc;
        }

        private async Task<List<News>> GetTheNewsDataAsync(HtmlWeb web, List<string> links)
        {
            var baseUrl = ServiceConstants.BaseUrl;

            var newsTasks = links.Select(async link =>
            {
                var doc = await web.LoadFromWebAsync(baseUrl + link);

                var titleElement = doc.DocumentNode.SelectSingleNode("//h1[@class='title']");
                if (titleElement != null)
                {
                    var title = titleElement.InnerText;

                    if (!await CheckIfNewsExistsAsync(title) && title != "")
                    {
                        string sterilizedContent;
                        var contentElement = doc.DocumentNode.SelectNodes("//div[@id='art_start']/p");
                        if (contentElement != null)
                        {
                            var nonEmptyParagraphs = contentElement.Where(p => !string.IsNullOrWhiteSpace(p.InnerText));
                            sterilizedContent = SterilizeTheNews(nonEmptyParagraphs);
                        }
                        else
                        {
                            sterilizedContent = "";
                        }

                        var dateElement = doc.DocumentNode.SelectSingleNode("//div[@class='art_author']");

                        string date = null;
                        var dateParsed = new DateTime(1000, 1, 1, 12, 30, 0);

                        if (dateElement != null)
                        {
                            if (dateElement.InnerText.Contains("|"))
                            {
                                var splitDates = dateElement.InnerText.Split("|");
                                var dateParts = splitDates.Last().Split(",");
                                date = dateParts.First().Trim();
                            }
                            else
                            {
                                var dateParts = dateElement.InnerText.Split(",");
                                date = dateParts.First().Trim();
                            }

                            try
                            {
                                if (!String.IsNullOrEmpty(date))
                                {
                                    if (date.Contains("мар"))
                                    {
                                        date = date.Replace("мар", "март");
                                    }
                                    dateParsed = DateTime.ParseExact(date, "d MMM yyyy HH:mm", new CultureInfo("bg-BG"));
                                }
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine($"Exception parsing the date! {e.Message}");
                                dateParsed = new DateTime(1000, 1, 1, 12, 30, 0);
                            }
                        }

                        var imageElement = doc.DocumentNode.SelectSingleNode("//div[@id='article_text']//img");
                        var imageSrc = imageElement != null ? imageElement.Attributes["src"].Value : ServiceConstants.DefaultNewsArticleImage;

                        var currentNews = new News
                        {
                            Title = title,
                            Content = sterilizedContent,
                            Date = dateParsed,
                            ImageUrl = imageSrc
                        };

                        return currentNews;
                    }
                }
                return null;
            });

            var news = (await Task.WhenAll(newsTasks)).Where(item => item != null).ToList();

            return news;
        }

        private async Task SaveNewsAsync(List<News> news)
        {
            var context = await data.CreateDbContextAsync();
            try
            {
                await context.News.AddRangeAsync(news);
                await context.SaveChangesAsync();
                foreach (var newsArticle in news)
                {
                    Console.WriteLine($"Saved news with title - {newsArticle.Title}");
                }
            }
            finally
            {
                await context.DisposeAsync();
            }
        }
 
        private async Task<bool> CheckIfNewsExistsAsync(string title)
        {
            var context = await data.CreateDbContextAsync();
            try
            {
                return await context
                    .News
                    .AnyAsync(n => n.Title == title);
            }
            finally
            {
                await context.DisposeAsync();
            }
        }

        private string SterilizeTheNews(IEnumerable<HtmlNode> contentParagraphs)
        {
            var paragraphsToCombine = new List<string>();
 
            foreach (var paragraph in contentParagraphs)
            {
                var sterilizedParagraph = RemoveHtmlTags(paragraph.InnerText);
 
                paragraphsToCombine.Add(sterilizedParagraph);
            }
 
            var combinedParagraphs = string.Join(Environment.NewLine + Environment.NewLine, paragraphsToCombine);
 
            return combinedParagraphs;
        }
 
        private string RemoveHtmlTags(string html)
        {
            var pattern = ServiceConstants.RegexPatternToRemoveHtmlTags;
 
            var strippedHtml = Regex.Replace(html, pattern, "");
 
            return strippedHtml;
        }
 
        public void Dispose()
        {
            _timer?.Dispose();
        }
 
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(Scrape, null, TimeSpan.Zero,
                TimeSpan.FromDays(10));
 
            return Task.CompletedTask;
        }
 
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }
    }
}