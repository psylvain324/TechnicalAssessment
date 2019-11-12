using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechnicalAssessment.Data;
using TechnicalAssessment.Models;
using TechnicalAssessment.Models.ViewModels;

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

        /// <summary>
        /// Get: All Transactions
        /// </summary>     
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Transactions()
        {
            return View(await databaseContext.Transactions.ToListAsync());
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
        [Route("/{transactionId:string}")]
        public async Task<IActionResult> TransactionById(string transactionId)
        {
            if (transactionId == null)
            {
                return BadRequest();
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
        /// GET: Transaction by Currency Code
        /// </summary>
        /// <param name="currencyCode"></param>
        /// <response code="200">If a valid request was made</response>
        /// <response code="404">If the transaction did not return a result</response>        
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("/ByCurrencyCode/{currencyCode}")]
        public async Task<IActionResult> TransactionByCurrencyCode(string currencyCode)
        {
            bool isValid = ValidCurrencyCode(currencyCode);
            if(isValid == false)
            {
                return BadRequest();
            }
            var transaction = await databaseContext.Transactions
                .SingleOrDefaultAsync(m => m.CurrencyCode == currencyCode);
            if (transaction == null)
            {
                return NotFound();
            }

            return View(transaction);
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

        // GET: Transactions/Create
        public IActionResult Create()
        {
            return View();
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
        [Route("/Create/{transaction}")]
        public async Task<IActionResult> CreateTransaction([Bind("TransactionId,Amount,CurrencyCode,TransactionDate,Status")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                databaseContext.Add(transaction);
                await databaseContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(transaction);
        }

        // GET: Transactions/UpdateTransaction/{transactionId}
        public async Task<IActionResult> UpdateTransaction(string transactionId)
        {
            if (transactionId == null)
            {
                return BadRequest();
            }

            var transaction = await databaseContext.Transactions.SingleOrDefaultAsync(m => m.TransactionId == transactionId);
            if (transaction == null)
            {
                return NotFound();
            }
            return View(transaction);
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
        [Route("/Update/{transaction}")]
        public async Task<IActionResult> UpdateTransactionAsync(string transactionId, [Bind("TransactionId,Amount,CurrencyCode,TransactionDate,Status")] Transaction transaction)
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
                    await databaseContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TransactionExists(transaction.TransactionId))
                    {
                        return NotFound();
                    }
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(transaction);
        }

        private bool TransactionExists(string transactionId)
        {
            return databaseContext.Transactions.Any(e => e.TransactionId == transactionId);
        }

        public bool ValidCurrencyCode(string currencyCode)
        {
            bool isvalid = false;
            var currencyCodes = databaseContext.Currencies;
            foreach (CurrencyViewModel currencyViewModel in currencyCodes)
            {
                foreach (Currency currency in currencyViewModel.CurrencyCodes)
                {
                    if (currency.Equals(currencyCode))
                    {
                        isvalid = true;
                    }
                }
            }
            return isvalid;
        }
    }
}
