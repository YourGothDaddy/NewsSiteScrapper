namespace NewsWebSiteScraper.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using NewsWebSiteScraper.Models;
    using NewsWebSiteScraper.Models.Home;
    using NewsWebSiteScraper.Services.Home;
    using System.Diagnostics;
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHomeService home;
        public HomeController(ILogger<HomeController> logger,
            IHomeService home)
        {
            _logger = logger;
            this.home = home;
        }
        public async Task<IActionResult> Index()
        {
            var recentNews = await home.RetrieveMostRecentNewsAsync(6);

            var model = new DisplayListOfMostRecentNewsViewModel
            {
                News = recentNews
            };

            return View(model);
        }
        public IActionResult Privacy()
        {
            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}