namespace NewsWebSiteScraper.Services.News
{
    public interface INewsService
    {
        public void SaveNews(string title,
            string imageUrl,
            string content);
    }
}
