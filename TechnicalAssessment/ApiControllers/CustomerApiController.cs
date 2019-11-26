using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Net.Mail;
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
        public IActionResult Customers()
        {
            return Ok(databaseContext.Customers.ToList());
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
        [Route("/GetByCustomerId/{customerId}")]
        public IActionResult CustomerById([FromRoute] int customerId)
        {
            var customer = databaseContext.Customers.Single(m => m.CustomerId == customerId);
            return Ok(customer);
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
        [Route("/GetByEmail/{email}")]
        public IActionResult CustomerByEmail([FromRoute] string email)
        {
            if (email == null || !email.Equals(typeof(MailAddress)))
            {
                return BadRequest();
            }

            var customer = databaseContext.Customers.Single(m => m.Email == email);
            if (customer == null)
            {
                return NotFound();
            }

            return Ok(customer);
        }

        /// <summary>
        /// GET: Customer like CustomerName or Email
        /// </summary>
        /// <param name="search"></param>
        /// <response code="200">If a valid request was made</response>
        /// <response code="404">If no Customers were not found</response>   
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("GetLikeNameOrEmail/{search}")]
        public IActionResult CustomerLikeNameOrEmail([FromRoute]string search)
        {
            var customers = from c in databaseContext.Customers select c;
            if (!string.IsNullOrEmpty(search))
            {
                customers = customers.Where(s => s.CustomerName.Contains(search) || s.Email.Contains(search));
            }
            if (customers == null || customers.Count<Customer>() < 1)
            {
                return NotFound();
            }

            return Ok(customers);
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
        public IActionResult CreateCustomer([Bind("CustomerId,CustomerName,Email,MobilePhone")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                databaseContext.Add(customer);
                databaseContext.SaveChanges();
                return CreatedAtRoute("Api/Customers/Create", customer);
            }
            return BadRequest(ModelState);
        }

        /// <summary>
        /// PATCH: Customer
        /// </summary>
        /// <param name="customer"></param>
        /// <param name="customerId"></param>
        /// <response code="201">Returns the newly updated Customer</response>
        /// <response code="400">If the Customer is null or invalid</response>
        /// <response code="409">If there is a concurrency exception</response>  
        [HttpPatch]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ValidateAntiForgeryToken]
        [Route("/Update/{customerId}")]
        public IActionResult UpdateCustomer([FromRoute]int customerId, [Bind("CustomerId,CustomerName,Email,MobilePhone")]  Customer customer)
        {
            if (customerId != customer.CustomerId)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    databaseContext.Update(customer);
                    databaseContext.SaveChanges();
                }
                catch (DbUpdateConcurrencyException e)
                {
                    if (!CustomerExists(customer.CustomerId))
                    {
                        return Conflict(e.InnerException);
                    }
                    return BadRequest();
                }
            }
            return CreatedAtRoute("Api/Customers/Update/{customerId}", customer);
        }

        /// <summary>
        /// DELETE: Customer
        /// </summary>
        /// <param name="customerId"></param>
        /// <response code="400">If the Customer is null or invalid</response>
        /// <response code="409">If there is a concurrency or database exception</response>  
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ValidateAntiForgeryToken]
        [Route("/Delete/{customerId}")]
        public IActionResult DeleteCustomer([FromRoute]int customerId)
        {
            if (ModelState.IsValid)
            {
                var customer = databaseContext.Customers.Single(m => m.CustomerId == customerId);
                try
                {
                    databaseContext.Customers.Remove(customer);
                    databaseContext.SaveChanges();
                }
                catch (DbUpdateException e)
                {
                    if (CustomerExists(customer.CustomerId))
                    {
                        return Conflict(e.InnerException);
                    }
                    return NotFound();
                }
            }
            return AcceptedAtAction("Api/Customers/Delete/{customerId}", customerId);
        }

        private bool CustomerExists(int customerId)
        {
            return databaseContext.Customers.Any(e => e.CustomerId == customerId);
        }
    }
}
