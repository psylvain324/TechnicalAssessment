using System;
using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using TechnicalAssessment.Services;

namespace TechnicalAssessment.Controllers
{
    public class UploadController : Controller
    {
        private TransactionService transactionService;
        private CustomerService customerService;
        private readonly ILogger<UploadController> logger;

        public UploadController(TransactionService transactionService, CustomerService customerService, ILogger<UploadController> logger)
        {
            this.transactionService = transactionService;
            this.customerService = customerService;
            this.logger = logger;
        }

        [HttpPost]
        public ActionResult Transaction(string filePath)
        {
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
                throw new NotSupportedException();
            }

            return RedirectPermanent("/Transaction/Details");
        }

        [HttpPost]
        public ActionResult Customer(string filePath)
        {
            return View();
        }
    }
}
