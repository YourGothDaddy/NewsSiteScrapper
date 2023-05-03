namespace NewsWebSiteScraper.Constants
{
    public class ServiceConstants
    {
        public static readonly string LastProcessedPageTxtName = "lastProcessedPage.txt";

        public static readonly string BaseUrl = "https://www.dnes.bg";

        public static readonly string DefaultNewsArticleImage = "https://digitalfinger.id/wp-content/uploads/2019/12/no-image-available-icon-6.png";

        public static readonly string RegexPatternToRemoveHtmlTags = @"<(?!strong(?:\s*(?!\s*\b(id|class)\b)[^>]*)?>|/strong(?:\s*(?!\s*\b(id|class)\b)[^>]*)?>)([^>]*)>|</(?!strong(?:\s*(?!\s*\b(id|class)\b)[^>]*)?>|/strong(?:\s*(?!\s*\b(id|class)\b)[^>]*)?>)[^>]*>";
    }
}
