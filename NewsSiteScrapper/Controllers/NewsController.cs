namespace NewsWebsiteSiteScraper.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using NewsWebSiteScraper.Services.News;

    using NewsWebSiteScraper.Constants;
    using NewsWebSiteScraper.Models.News;

    public class NewsController : Controller
    {
        private readonly INewsService news;

        public NewsController(INewsService news)
        {
            this.news = news;
        }
        public async Task<IActionResult> Bulgaria(int? pageNumber)
        {
            var numberOfNewsOnPage = ControllerConstants.NumberOfNewsOnAPage;
            pageNumber ??= 1;
            var allNews = await this.news.RetrieveAllNewsCountAsync();

            if (pageNumber < 1)
            {
                return RedirectToAction("Bulgaria", new { pageNumber = 1 });
            }

            var newsOnThePage = await this.news.RetrieveAllNewsForThePageAsync(pageNumber, numberOfNewsOnPage);
            var totalPages = (int)Math.Ceiling((double)allNews / numberOfNewsOnPage);

            if (pageNumber > totalPages)
            {
                return RedirectToAction("Bulgaria", new { pageNumber = totalPages });
            }

            var viewModel = new DisplayListOfNewsViewModel
            {
                News = newsOnThePage,
                PageNumber = pageNumber.Value,
                NumberOfNewsOnPage = numberOfNewsOnPage,
                TotalNewsCount = allNews,
                TotalPages = totalPages
            };

            return View(viewModel);
        }
        public async Task<IActionResult> NewsDetails(int id)
        {
            var newsItem = await this.news.RetrieveNewsAsync(id);

            if (newsItem == null)
            {
                return NotFound();
            }

            var viewModel = new NewsDetailsViewModel
            {
                News = newsItem
            };

            return View(viewModel);
        }

    }
}
