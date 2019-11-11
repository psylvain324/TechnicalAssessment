using System.Net.Mail;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TechnicalAssessment.Data;
using TechnicalAssessment.Models;

namespace TechnicalAssessment.Controllers
{
    [Produces("application/json")]
    [Route("Customers")]
    [ApiController]
    public class CustomerController : Controller
    {
        private DatabaseContext databaseContext;

        public CustomerController(DatabaseContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var customers = databaseContext.Customers.ToList();
            return View(customers);
        }

        /// <summary>
        /// Gets all Customers
        /// </summary>     
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Route("/All")]
        public async Task<IActionResult> GetAllCustomers()
        {
            return View(await databaseContext.Customers.ToListAsync());
        }

        /// <summary>
        /// Returns a Customer record by Customer Id
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns>A Customer record</returns>
        /// <response code="200">If a valid request was made</response>
        /// <response code="400">If the Customer is null or invalid</response>     
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Route("/CustomerById/{customerId}")]
        public async Task<IActionResult> GetCustomerById(int customerId)
        {
            var customer = await databaseContext.Customers
                .SingleOrDefaultAsync(m => m.CustomerId == customerId);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        /// <summary>
        /// Returns a Customer record by Email
        /// </summary>
        /// <param name="email"></param>
        /// <returns>A Customer record</returns>
        /// <response code="200">If a valid request was made</response>
        /// <response code="400">If the Customer is null</response>      
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Route("/CustomerByEmail/{email}")]
        public async Task<IActionResult> GetCustomerByEmail(string email)
        {
            if (!email.Equals(typeof(MailAddress)))
            {
                return BadRequest();
            }

            var customer = await databaseContext.Customers
                .SingleOrDefaultAsync(m => m.Email == email);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        /// <summary>
        /// Returns a Customer record by Id and Email
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="email"></param>
        /// <returns>A Customer record</returns>
        /// <response code="200">If a valid request was made</response>
        /// <response code="400">If the Customer is null</response>            
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Route("/CustomerByIdAndEmail/{customerId}/{email}")]
        public IActionResult GetCustomerByIdAndEmail(int customerId, string email)
        {
            if (!customerId.Equals(typeof(int)) || !email.Equals(typeof(MailAddress)))
            {
                return BadRequest();
            }

            var customers = databaseContext.Customers.Select(c =>
                new { customerId, email }).ToList();
            if (customers == null)
            {
                return NotFound();
            }

            return View(customers);
        }

        /// <summary>
        /// Creates a Customer record.
        /// </summary>
        /// <param name="customer"></param>
        /// <returns>A newly created Customer</returns>
        /// <response code="201">Returns the newly created Customer</response>
        /// <response code="400">If the Customer is null or invalid</response>            
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Route("/Create/{customer}")]
        public ActionResult<Customer> CreateCustomer(Customer customer)
        {
            databaseContext.Customers.Add(customer);
            databaseContext.SaveChanges();
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Updates a Customer record.
        /// </summary>
        /// <param name="customer"></param>
        /// <returns>A newly updated Customer</returns>
        /// <response code="201">Returns the newly updated Customer</response>
        /// <response code="400">If the Customer is null or invalid</response>            
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Route("/Update/{customer}")]
        public IActionResult UpdateCustomer(Customer customer)
        {
            databaseContext.Customers.Update(customer);
            databaseContext.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
