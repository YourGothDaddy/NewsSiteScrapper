namespace NewsWebsiteSiteScraper.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using NewsWebSiteScraper.Services.News;

    using NewsWebSiteScraper.Constants;
    using NewsWebSiteScraper.Models.News;
    using NewsWebSiteScraper.Services.Users;
    using NewsWebSiteScraper.Data.Models;
    using System.Security.Claims;

    public class NewsController : Controller
    {
        private readonly INewsService news;
        private readonly IUserService users;

        public NewsController(INewsService news,
            IUserService users)
        {
            this.news = news;
            this.users = users;
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

           
            var userId = HttpContext.User.Identity.Name;
            Console.WriteLine(userId);
            if (userId == null)
            {
                Console.WriteLine("We should not be here!");
                userId = HttpContext.Connection.RemoteIpAddress.ToString();
            }

            var userHasViewedTheNews = await this.users.UserHasViewedTheNewsAsync(id, userId);

            if (!userHasViewedTheNews)
            {
                await this.news.IncrementUniqueNewsAsync(id, userId);
            }

            var comments = await this.news.RetrieveCommentsAsync(id);

            var viewModel = new NewsDetailsViewModel
            {
                News = newsItem,
                Comments = comments
            };

            return View(viewModel);
        }

        public async Task<IActionResult> AddComment(int newsId, string commentContent)
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return BadRequest("User is not authenticated");
            }

            var comment = new Comment
            {
                NewsId = newsId,
                Content = commentContent,
                Date = DateTime.UtcNow,
                UserId = userId
            };

            await this.news.AddCommentAsync(comment);

            return RedirectToAction("NewsDetails", new { id = newsId });
        }

    }
}
