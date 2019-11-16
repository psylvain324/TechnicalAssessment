using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TechnicalAssessment.Data;
using TechnicalAssessment.Services.Interfaces;

namespace TechnicalAssessment.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ILogger<TransactionController> logger;
        private readonly DatabaseContext databaseContext;
        private IServiceUpload customerService;

        public CustomerController(ILogger<TransactionController> logger, DatabaseContext databaseContext, IServiceUpload customerService)
        {
            this.logger = logger;
            this.databaseContext = databaseContext;
            this.customerService = customerService;
        }

        //GET: Customer
        public async Task<IActionResult> CustomerIndex()
        {
            return View(await databaseContext.Customers.ToListAsync().ConfigureAwait(false));
        }

        //GET: Customer/Details/{id}
        [Route("CustomerDetails/{id}")]
        public async Task<IActionResult> CustomerDetails(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await databaseContext.Customers.FirstOrDefaultAsync().ConfigureAwait(false);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // GET: Customer/Edit/{id}
        [Route("CustomerEdit/{id}")]
        public async Task<IActionResult> CustomerEdit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customers = await databaseContext.Customers.SingleOrDefaultAsync(m => m.CustomerId == id).ConfigureAwait(false);
            if (customers == null)
            {
                return NotFound();
            }
            return View(customers);
        }

        //GET Customer/CustomerSearch/{search}/{field}
        [Route("CustomerSearch/{search}/{field}")]
        public IActionResult CustomerSearch(string search, string field)
        {
            CultureInfo culture = new CultureInfo(CultureInfo.CurrentCulture.Name);
            var customers = from c in databaseContext.Customers select c;
            switch (field)
            {
                case "CustomerID":
                    customers = from c in databaseContext.Customers
                                   where c.CustomerId == int.Parse(search, culture.NumberFormat)
                                   select c;
                    break;
                case "Email":
                    customers = from c in databaseContext.Customers
                                   where c.Email == search
                                   select c;
                    break;
                default:
                    return NotFound();
            }

            return View(customers);
        }

        [HttpPost]
        [Route("/UploadCustomer/{file}")]
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
                customerService.UploadCsv(filePath);
            }
            else if (Path.GetExtension(filePath) == "xml")
            {
                customerService.UploadXml(filePath);
            }
            else
            {
                return BadRequest();
            }

            return RedirectToAction("CustomerIndex", "Customer");
        }
    }
}
