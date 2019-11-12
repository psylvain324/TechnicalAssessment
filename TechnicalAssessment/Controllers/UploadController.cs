using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TechnicalAssessment.Data;
using TechnicalAssessment.Services;

namespace TechnicalAssessment.Controllers
{
    [Produces("application/json")]
    [Route("Uploads")]
    [ApiController]
    public class UploadController : Controller
    {
        private TransactionService transactionService;
        private CustomerService customerService;

        public UploadController(TransactionService transactionService, CustomerService customerService)
        {
            this.transactionService = transactionService;
            this.customerService = customerService;
        }

        [Route("/Csv")]
        [HttpPost]
        public ActionResult UploadCsv(string filePath)
        {
            //TODO
            return View();
        }

        [Route("/Xml")]
        [HttpPost]
        public ActionResult UploadXml(string filePath)
        {
            //TODO
            return View();
        }
    }
}
