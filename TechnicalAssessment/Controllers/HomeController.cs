using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TechnicalAssessment.Models.ViewModels;

namespace TechnicalAssessment.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> logger;

        public HomeController(ILogger<HomeController> logger)
        {
            this.logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Transactions()
        {
            return View(new TransactionViewModel());
        }

        public IActionResult Customers()
        {
            return View(new CustomerViewModel());
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
