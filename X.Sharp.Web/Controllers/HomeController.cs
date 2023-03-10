using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.Extensions.Localization;
using X.Sharp.Web.Example;
using X.Sharp.Web.Models;
using Microsoft.AspNetCore.Localization;

namespace X.Sharp.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IFoo _foo;
        private readonly IStringLocalizer<Globals> _stringLocalizer;
        public HomeController(ILogger<HomeController> logger, IFoo foo, IStringLocalizer<Globals> stringLocalizer)
        {
            _logger = logger;
            _foo = foo;
            _stringLocalizer = stringLocalizer;
        }

        public IActionResult Index()
        {
            _foo.Print("Index Page");
            ViewData["Title"] = _stringLocalizer["Title"];
            var culture = this.HttpContext.Features.Get<IRequestCultureFeature>();
            ViewData["Culture"] = culture.RequestCulture.Culture;
            ViewData["UICulture"] = culture.RequestCulture.UICulture;
            return View();
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