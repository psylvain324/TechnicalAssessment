using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TechnicalAssessment.Data;
using TechnicalAssessment.Models;
using TechnicalAssessment.Models.ViewModels;
using TechnicalAssessment.Services.Interfaces;

namespace TechnicalAssessment.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ILogger<TransactionController> logger;
        private readonly DatabaseContext databaseContext;
        private readonly IFormatProvider formatProvider;
        private readonly IServiceUpload<Customer> customerServiceUpload;

        public CustomerController(ILogger<TransactionController> logger, DatabaseContext databaseContext, IServiceUpload<Customer> customerServiceUpload)
        {
            this.logger = logger;
            this.databaseContext = databaseContext;
            formatProvider = CultureInfo.CreateSpecificCulture(CultureInfo.CurrentCulture.ThreeLetterISOLanguageName);
            this.customerServiceUpload = customerServiceUpload;
        }

        // GET: Customer
        public async Task<IActionResult> CustomerIndex(string sortOrder, string currentFilter, string searchString, int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "Name";
            ViewData["EmailSortParm"] = string.IsNullOrEmpty(sortOrder) ? "email_desc" : "Email";

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;

            var customers = from c in databaseContext.Customers select c;
            if (!string.IsNullOrEmpty(searchString))
            {
                customers = customers.Where(s => s.CustomerName.Contains(searchString) || s.Email.Contains(searchString) || s.CustomerId.Equals(int.Parse(searchString, formatProvider)));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    customers = customers.OrderByDescending(s => s.CustomerName);
                    break;
                case "Name":
                    customers = customers.OrderBy(s => s.CustomerName);
                    break;
                case "email_desc":
                    customers = customers.OrderByDescending(s => s.Email);
                    break;
                case "Email":
                    customers = customers.OrderByDescending(s => s.Email);
                    break;
                default:
                    customers = customers.OrderBy(s => s.CustomerId);
                    break;
            }

            int pageSize = 5;
            return View(await PaginatedList<Customer>.CreateAsync(customers.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        //GET: Customer/Details/{id}
        [Route("CustomerDetails/{id}")]
        public async Task<IActionResult> CustomerDetails(string id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            var customer = await databaseContext.Customers.FirstOrDefaultAsync().ConfigureAwait(false);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        //GET: Customer/CustomerDelete/{id}
        public async Task<IActionResult> CustomerDelete(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            var customer = await databaseContext.Customers.SingleOrDefaultAsync(m => m.CustomerId == id).ConfigureAwait(false);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        //POST: Customer/CustomerDelete/{id}
        [HttpPost, ActionName("CustomerDelete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CustomerDeleteConfirmed(int id)
        {
            var customer = await databaseContext.Customers.SingleOrDefaultAsync(m => m.CustomerId == id).ConfigureAwait(false);
            databaseContext.Customers.Remove(customer);
            await databaseContext.SaveChangesAsync().ConfigureAwait(false);
            return RedirectToAction("CustomerIndex");
        }

        public IActionResult CustomerCreate()
        {
            return View();
        }

        //POST: Customer/CustomerCreate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CustomerCreate(Customer customer)
        {
            if (ModelState.IsValid)
            {
                databaseContext.Customers.Add(customer);
                await databaseContext.SaveChangesAsync().ConfigureAwait(false);
                return RedirectToAction("CustomerIndex");
            }
            return View(customer);
        }

        //GET: Customer/Edit/{id}
        [Route("CustomerEdit/{id}")]
        public async Task<IActionResult> CustomerEdit(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            var customers = await databaseContext.Customers.SingleOrDefaultAsync(m => m.CustomerId == id).ConfigureAwait(false);
            if (customers == null)
            {
                return NotFound();
            }
            return View(customers);
        }

        //POST: Transaction/UploadCustomer/{file}
        [HttpPost]
        [Route("/UploadCustomer/{file}")]
        public ActionResult UploadTransaction(IFormFile file)
        {
            if (file != null)
            {
                if (file.Length > 1000000)
                {
                    logger.LogInformation("Request was either Null or File Size was too large. File was: " + file.Length + " Bytes.");
                    return BadRequest();
                }
                if (Path.GetExtension(file.Name) == ".csv")
                {
                    customerServiceUpload.UploadCsv(file);
                }
                else if (Path.GetExtension(file.Name) == ".xml")
                {
                    customerServiceUpload.UploadXml(file);
                }
                else
                {
                    return BadRequest();
                }
            }

            return RedirectToAction("CustomerIndex", "Customer");
        }
    }
}
