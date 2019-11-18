using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using TechnicalAssessment.Data;
using TechnicalAssessment.Models;

namespace TechnicalAssessment.ApiControllers
{
    [Produces("application/json")]
    [Route("Api/Customers")]
    [ApiController]
    public class CustomersController : Controller
    {
        private readonly DatabaseContext databaseContext;

        public CustomersController(DatabaseContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }

        /// <summary>
        /// Get: All Customers
        /// </summary>     
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Route("/Get/All")]
        public async Task<IActionResult> Customers()
        {
            return View(await databaseContext.Customers.ToListAsync().ConfigureAwait(false));
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
        [Route("/Get/{customerId}")]
        public async Task<IActionResult> CustomerById([FromRoute] int customerId)
        {
            var customer = await databaseContext.Customers
                .SingleOrDefaultAsync(m => m.CustomerId == customerId).ConfigureAwait(false);
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
        [Route("/Email/{email}")]
        public async Task<IActionResult> CustomerByEmail([FromRoute] string email)
        {
            if (!email.Equals(typeof(MailAddress)))
            {
                return BadRequest();
            }

            var customer = await databaseContext.Customers
                .SingleOrDefaultAsync(m => m.Email == email).ConfigureAwait(false);
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
        [Route("/Get/{customerId}/{email}")]
        public IActionResult CustomerByIdAndEmail([FromRoute]int customerId, [FromRoute] string email)
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
        /// PUT: Customer
        /// </summary>
        /// <param name="customer"></param>
        /// <response code="201">Returns the newly created Customer</response>
        /// <response code="400">If the Customer is null or invalid</response>            
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ValidateAntiForgeryToken]
        [Route("/Create")]
        public async Task<IActionResult> CreateCustomer([Bind("CustomerId,CustomerName,Email,MobilePhone")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                databaseContext.Add(customer);
                await databaseContext.SaveChangesAsync().ConfigureAwait(false);
                return RedirectToAction(nameof(Index));
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
        [Route("/Update/{customerId}")]
        public async Task<IActionResult> UpdateCustomerAsync([FromRoute]int customerId, [Bind("CustomerId,CustomerName,Email,MobilePhone")]  Customer customer)
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
                    await databaseContext.SaveChangesAsync().ConfigureAwait(false);
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
