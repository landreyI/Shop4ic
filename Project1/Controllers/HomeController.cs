using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Project1.Models;
using Project1.Models.DBModels;
using Project1.Service;
using System.Diagnostics;
using System.Security.Claims;

namespace Project1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DBService bdService;
        public HomeController(ILogger<HomeController> logger, DBService _bdService)
        {
            _logger = logger;
            bdService = _bdService;
        }

        public IActionResult Index()
        {   

            ListCategory.AddCategores(bdService.GetAllCategory());
            return View(bdService.GetAllCategory());
        }
        public IActionResult Info()
        {
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
