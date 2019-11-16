using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using TechnicalAssessment.Services;
using Microsoft.AspNetCore.Http;

namespace TechnicalAssessment.Controllers
{
    [Route("Upload")]
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
        [Route("/UploadTransaction/{file}")]
        public ActionResult UploadTransaction(IFormFile file)
        {
            if (file == null || file.Length > 1000000)
            {
                logger.LogInformation("Request was either Null or File Size was too large. File was: " + file.Length + " Bytes.");
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

            return RedirectToAction("Index", "Transaction");
        }

        [HttpPost]
        public ActionResult UploadCustomer(IFormFile file)
        {
            return View();
        }
    }
}
