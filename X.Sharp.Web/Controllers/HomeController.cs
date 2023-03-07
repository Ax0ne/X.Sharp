using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using X.Sharp.Web.Example;
using X.Sharp.Web.Models;

namespace X.Sharp.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        IFoo _foo;
        public HomeController(ILogger<HomeController> logger, IFoo foo)
        {
            _logger = logger;
            this._foo = foo;
        }

        public IActionResult Index()
        {
            _foo.Print("Index Page");
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