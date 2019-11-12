using System.Net.Mail;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TechnicalAssessment.Data;
using TechnicalAssessment.Models;
using System;
using Microsoft.EntityFrameworkCore;

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

        /// <summary>
        /// Get: All Customers
        /// </summary>     
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Customers()
        {
            return View(await databaseContext.Customers.ToListAsync());
        }

        /// <summary>
        /// GET: Customer by CustomerId
        /// </summary>
        /// <param name="customerId"></param>
        /// <response code="200">If a valid request was made</response>
        /// <response code="404">If the transaction did not return a result</response>      
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("/{customerId:int}")]
        public async Task<IActionResult> CustomerById(int customerId)
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
        /// GET: Customer by Email
        /// </summary>
        /// <param name="email"></param>
        /// <response code="200">If a valid request was made</response>
        /// <response code="400">If the Customer is null</response>
        /// <response code="404">If the Customer was not found</response>   
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("/ByEmail/{email:string}")]
        public async Task<IActionResult> CustomerByEmail(string email)
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
        /// GET: Customer by CustomerId and Email
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="email"></param>
        /// <response code="200">If a valid request was made</response>
        /// <response code="400">If the CustomerID or Email is null or invalid</response>
        /// <response code="404">If the Customer was not found</response>   
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("/{customerId:int}/{email:string}")]
        public IActionResult CustomerByIdAndEmail(int customerId, string email)
        {
            if (!customerId.Equals(typeof(int)) || !email.Equals(typeof(MailAddress)))
            {
                return BadRequest();
            }

            var customer = databaseContext.Customers.Select(c =>
                new { customerId, email }).FirstOrDefault();
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        /// <summary>
        /// POST: Customer
        /// </summary>
        /// <param name="customer"></param>
        /// <response code="201">Returns the newly created Customer</response>
        /// <response code="400">If the Customer is null or invalid</response>            
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ValidateAntiForgeryToken]
        [Route("/Create/{customer}")]
        public async Task<IActionResult> CreateCustomer([Bind("CustomerId,CustomerName,Email,MobilePhone")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                databaseContext.Add(customer);
                await databaseContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        // GET: Customers/UpdateCustomer/{customerId}
        public async Task<IActionResult> UpdateCustomer(int customerId)
        {
            var customer = await databaseContext.Customers.SingleOrDefaultAsync(m => m.CustomerId == customerId);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        /// <summary>
        /// PATCH: Customer
        /// </summary>
        /// <param name="customer"></param>
        /// <response code="201">Returns the newly updated Customer</response>
        /// <response code="400">If the Customer is null or invalid</response>            
        [HttpPatch]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ValidateAntiForgeryToken]
        [Route("/Update/{customer}")]
        public async Task<IActionResult> UpdateCustomerAsync(int customerId, [Bind("CustomerId,CustomerName,Email,MobilePhone")]  Customer customer)
        {
            if (customerId != customer.CustomerId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    databaseContext.Update(customer);
                    await databaseContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerExists(customer.CustomerId))
                    {
                        return NotFound();
                    }
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        private bool CustomerExists(int customerId)
        {
            return databaseContext.Customers.Any(e => e.CustomerId == customerId);
        }
    }
}
