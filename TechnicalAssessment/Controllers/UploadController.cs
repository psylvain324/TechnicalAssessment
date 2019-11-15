using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using TechnicalAssessment.Services;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace TechnicalAssessment.Controllers
{
    public class UploadController : Controller
    {
        private readonly TransactionService transactionService;
        private readonly CustomerService customerService;
        private readonly ILogger<UploadController> logger;

        public UploadController(TransactionService transactionService, CustomerService customerService, ILogger<UploadController> logger)
        {
            this.transactionService = transactionService;
            this.customerService = customerService;
            this.logger = logger;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult Transaction(IFormFile file)
        {
            if (file.Length == 0 || file.Length > 1000000)
            {
                return BadRequest();
            }
            var filePath = Path.GetTempFileName();
            if (Path.GetExtension(filePath) == "csv")
            {
                transactionService.UploadCsv(filePath);
            }
            else if (Path.GetExtension(filePath) == "xml")
            {
                transactionService.UploadXml(filePath);
            }
            else
            {
                return BadRequest();
            }

            return RedirectPermanent("/Transaction/Index");
        }

        [HttpPost]
        public ActionResult Customer(IFormFile file)
        {
            return View();
        }
    }
}
