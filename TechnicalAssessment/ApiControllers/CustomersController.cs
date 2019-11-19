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
        [Route("/GetById/{customerId}")]
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
        [Route("/GetByIdAndEmail/{customerId}/{email}")]
        public IActionResult CustomerByIdAndEmail([FromRoute]int customerId, [FromRoute] string email)
        {
            if (!customerId.Equals(typeof(int)) || email == null || !email.Equals(typeof(MailAddress)))
            {
                return BadRequest();
            }

            var customer = databaseContext.Customers.Select(c =>
                new { customerId, email }).FirstOrDefault();
            if (customer == null)
            {
                return NotFound();
            }

            return Ok(customer);
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
        /// <response code="201">Returns the newly updated Customer</response>
        /// <response code="400">If the Customer is null or invalid</response>            
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

        private bool CustomerExists(int customerId)
        {
            return databaseContext.Customers.Any(e => e.CustomerId == customerId);
        }
    }
}
