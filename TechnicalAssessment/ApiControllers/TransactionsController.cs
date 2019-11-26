using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechnicalAssessment.Data;
using TechnicalAssessment.Models;
using TechnicalAssessment.Models.ViewModels;

namespace TechnicalAssessment.ApiControllers
{
    [Produces("application/json")]
    [Route("Api/Transactions")]
    [ApiController]
    public class TransactionsController : Controller
    {
        private readonly DatabaseContext databaseContext;

        public TransactionsController(DatabaseContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }

        /// <summary>
        /// Get: All Transactions
        /// </summary>     
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Route("/Get/All")]
        public IActionResult Transactions()
        {
            return Ok(databaseContext.Transactions.ToList());
        }

        /// <summary>
        /// GET: Transaction by TransactionId
        /// </summary>
        /// <param name="transactionId"></param>
        /// <response code="200">If a valid request was made</response>
        /// <response code="404">If the Transaction was not found</response>     
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("/GetById/{transactionId}")]
        public IActionResult TransactionById([FromRoute]string transactionId)
        {
            if (transactionId == null)
            {
                return BadRequest();
            }

            var transaction = databaseContext.Transactions.Single(m => m.TransactionId == transactionId);
            if (transaction == null)
            {
                return NotFound();
            }

            return Ok(transaction);
        }

        /// <summary>
        /// GET: Transaction by Currency Code
        /// </summary>
        /// <param name="currencyCode"></param>
        /// <response code="200">If a valid request was made</response>
        /// <response code="404">If the transaction did not return a result</response>        
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("/GetByCurrencyCode/{currencyCode}")]
        public IActionResult TransactionByCurrencyCode([FromRoute]string currencyCode)
        {
            bool isValid = ValidCurrencyCode(currencyCode);
            if(isValid == false)
            {
                return BadRequest();
            }
            var transaction = databaseContext.Transactions.Single(m => m.CurrencyCode == currencyCode);
            if (transaction == null)
            {
                return NotFound();
            }

            return Ok(transaction);
        }

        /// <summary>
        /// GET: Transaction by Status
        /// </summary>
        /// <param name="transactionStatus"></param>
        /// <response code="201">Returns the newly created transaction</response>
        /// <response code="400">If the Transaction Request is null or invalid</response>
        /// <response code="404">If the transaction did not return a result</response>    
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("/GetByStatus/{transactionStatus}")]
        public IActionResult TransactionByStatus([FromRoute]TransactionStatus transactionStatus)
        {
            if (transactionStatus.ToString() == null)
            {
                return BadRequest();
            }

            var transaction = databaseContext.Transactions.Single(m => m.Status == transactionStatus);
            if (transaction == null)
            {
                return NotFound();
            }

            return Ok(transaction);
        }

        /// <summary>
        /// POST: Transaction Create
        /// </summary>
        /// <param name="transaction"></param>
        /// <response code="201">Returns the newly created Transaction</response>
        /// <response code="400">If the Transaction is null or invalid</response>            
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ValidateAntiForgeryToken]
        [Route("/Create")]
        public IActionResult CreateTransaction([Bind("TransactionId,Amount,CurrencyCode,TransactionDate,Status")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                databaseContext.Add(transaction);
                databaseContext.SaveChangesAsync();
                return CreatedAtRoute("Api/Transactions/Create", transaction);
            }
            return BadRequest(ModelState);
        }

        /// <summary>
        /// PATCH: Update Transaction
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="transactionId"></param>
        /// <response code="201">Returns the newly updated Transaction</response>
        /// <response code="400">If the Transaction is null or invalid</response>    
        [HttpPatch]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ValidateAntiForgeryToken]
        [Route("/Update/{transactionId}")]
        public IActionResult UpdateTransaction([FromRoute]string transactionId, [Bind("TransactionId,Amount,CurrencyCode,TransactionDate,Status")] Transaction transaction)
        {
            if (transactionId != transaction.TransactionId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    databaseContext.Update(transaction);
                    databaseContext.SaveChanges();
                }
                catch (DbUpdateConcurrencyException e)
                {
                    if (!TransactionExists(transaction.TransactionId))
                    {
                        return NotFound();
                    }
                    return BadRequest(e.InnerException);
                }
                return CreatedAtRoute("Api/Transactions/Update/{transactionId}", transaction);
            }
            return BadRequest(ModelState);
        }

        private bool TransactionExists(string transactionId)
        {
            return databaseContext.Transactions.Any(e => e.TransactionId == transactionId);
        }

        public bool ValidCurrencyCode(string currencyCode)
        {
            bool isvalid = false;
            var currencyCodes = databaseContext.Currencies;
            foreach (Currency currency in currencyCodes)
            {
                isvalid |= currency.Equals(currencyCode);
            }
            return isvalid;
        }
    }
}
