using System;
using System.Data.Entity;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TechnicalAssessment.Data;
using TechnicalAssessment.Models;

namespace TechnicalAssessment.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : Controller
    {
        private readonly DatabaseContext databaseContext;

        public TransactionController(DatabaseContext context)
        {
            databaseContext = context;
        }

        /// <summary>
        /// Gets all Transactions
        /// </summary>     
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Index()
        {
            return View(await databaseContext.Transactions.ToListAsync());
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blog = await databaseContext.Transactions
                .SingleOrDefaultAsync(m => m.TransactionId == id);
            if (blog == null)
            {
                return NotFound();
            }

            return View(blog);
        }

        /// <summary>
        /// Creates a Transaction record.
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns>A newly created Transaction</returns>
        /// <response code="201">Returns the newly created transaction</response>
        /// <response code="400">If the transaction is null</response>            
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<Transaction> Create(Transaction transaction)
        {
            databaseContext.Transactions.Add(transaction);
            databaseContext.SaveChanges();

            return CreatedAtRoute("GetTransaction", new { id = transaction.TransactionId }, transaction);
        }
    }
}
