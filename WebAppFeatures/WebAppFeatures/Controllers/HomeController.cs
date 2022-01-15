using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using WebAppFeatures.Models;

namespace WebAppFeatures.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> logger;
        public HomeController(ILogger<HomeController> _logger)
        {
            logger = _logger;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }
        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {

            logger.LogInformation($"SetLanguage=> culture:{culture}, returnUrl:{returnUrl} ");
            Response.Cookies.Append(
            CookieRequestCultureProvider.DefaultCookieName,
            CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)), 
            new Microsoft.AspNetCore.Http.CookieOptions { Expires = DateTime.UtcNow.AddYears(1)}); 
            return LocalRedirect(returnUrl);
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
