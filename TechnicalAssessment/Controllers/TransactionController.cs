using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TechnicalAssessment.Data;
using TechnicalAssessment.Models;

namespace TechnicalAssessment.Controllers
{
    [Produces("application/json")]
    [Route("Transactions")]
    [ApiController]
    public class TransactionController : Controller
    {
        private DatabaseContext databaseContext;

        public TransactionController(DatabaseContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var transactions = databaseContext.Transactions.ToList();
            return View(transactions);
        }

        /// <summary>
        /// Gets all Transactions
        /// </summary>     
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Route("/All")]
        public async Task<IActionResult> GetAllTransactions()
        {
            return View(await databaseContext.Transactions.ToListAsync());
        }

        /// <summary>
        /// Returns a Transaction record by transaction Id
        /// </summary>
        /// <param name="transactionId"></param>
        /// <returns>A Transaction record</returns>
        /// <response code="200">If a valid request was made</response>
        /// <response code="400">If the transaction is null or invalid</response>     
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Route("/TransactionById/{transactionId}")]
        public async Task<IActionResult> TransactionById(string transactionId)
        {
            if (transactionId == null)
            {
                return NotFound();
            }

            var transaction = await databaseContext.Transactions
                .SingleOrDefaultAsync(m => m.TransactionId == transactionId);
            if (transaction == null)
            {
                return NotFound();
            }

            return View(transaction);
        }

        /// <summary>
        /// Returns a Transaction record by transaction currency code
        /// </summary>
        /// <param name="currencyCode"></param>
        /// <returns>A Transaction record</returns>
        /// <response code="201">Returns the newly created transaction</response>
        /// <response code="400">If the transaction is null</response>      
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Route("/TransactionByCurrencyCode/{currencyCode}")]
        public async Task<IActionResult> TransactionByCurrencyCode(string currencyCode)
        {
            var transaction = await databaseContext.Transactions
                .SingleOrDefaultAsync(m => m.CurrencyCode == currencyCode);
            if (transaction == null)
            {
                return NotFound();
            }

            return View(transaction);
        }

        /// <summary>
        /// Returns a Transaction record by transaction status
        /// </summary>
        /// <param name="transactionStatus"></param>
        /// <returns>A Transaction record</returns>
        /// <response code="201">Returns the newly created transaction</response>
        /// <response code="400">If the Transaction is null</response>            
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Route("/TransactionByStatus/{transactionStatus}")]
        public async Task<IActionResult> TransactionByStatus(TransactionStatus transactionStatus)
        {
            if (transactionStatus.ToString() == null)
            {
                return BadRequest();
            }

            var transaction = await databaseContext.Transactions
                .SingleOrDefaultAsync(m => m.Status == transactionStatus);
            if (transaction == null)
            {
                return NotFound();
            }

            return View(transaction);
        }

        /// <summary>
        /// Creates a Transaction record.
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns>A newly created Transaction</returns>
        /// <response code="201">Returns the newly created Transaction</response>
        /// <response code="400">If the Transaction is null</response>            
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Route("/Create/{transaction}")]
        public ActionResult<Transaction> CreateTransaction(Transaction transaction)
        {
            databaseContext.Transactions.Add(transaction);
            databaseContext.SaveChanges();
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Updates a Transaction record.
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns>A newly created Transaction</returns>
        /// <response code="201">Returns the newly updated Transaction</response>
        /// <response code="400">If the Transaction is null or invalid</response>    
        [HttpPost]
        [Route("/Update/{transaction}")]
        public IActionResult UpdateTransaction(Transaction transaction)
        {
            databaseContext.Transactions.Update(transaction);
            databaseContext.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
