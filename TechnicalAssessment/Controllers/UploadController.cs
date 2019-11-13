using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TechnicalAssessment.Data;
using TechnicalAssessment.Services;

namespace TechnicalAssessment.Controllers
{
    [Produces("application/json")]
    [Route("api/Uploads")]
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
        public ActionResult Upload(string filePath)
        {
            string extensionType = Path.GetExtension(filePath);
            try
            {
                if (extensionType == "csv")
                {

                }
                else if (extensionType == "xml")
                {

                }
            }
            catch (Exception)
            {
                throw new Exception();
            }

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
