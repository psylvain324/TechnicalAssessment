using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TechnicalAssessment.Data;
using TechnicalAssessment.Models;
using TechnicalAssessment.Services.Interfaces;

namespace TechnicalAssessment.Controllers
{
    public class TransactionController : Controller
    {
        private readonly ILogger<TransactionController> logger;
        private readonly DatabaseContext databaseContext;
        private readonly IServiceUpload transactionService;

        public TransactionController(ILogger<TransactionController> logger, DatabaseContext databaseContext, IServiceUpload transactionService)
        {
            this.logger = logger;
            this.databaseContext = databaseContext;
            this.transactionService = transactionService;
        }

        //GET: Transaction
        public async Task<IActionResult> TransactionIndex()
        {
            return View(await databaseContext.Transactions.ToListAsync().ConfigureAwait(false));
        }

        //GET: Transaction/TransactionDetails/{id}
        [Route("/TransactionDetails/{id}")]
        public async Task<IActionResult> TransactionDetails(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transaction = await databaseContext.Transactions.FirstOrDefaultAsync().ConfigureAwait(false);
            if (transaction == null)
            {
                return NotFound();
            }

            return View(transaction);
        }

        //GET: Transaction/TransactionDelete/{id}
        public async Task<IActionResult> TransactionDelete(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            var transaction = await databaseContext.Transactions.SingleOrDefaultAsync(m => m.Id == id).ConfigureAwait(false);
            if (transaction == null)
            {
                return NotFound();
            }

            return View(transaction);
        }

        //POST: Transaction/TransactionDelete/{id}
        [HttpPost, ActionName("TransactionDelete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TransactionConfirmDelete(int id)
        {
            var transaction = await databaseContext.Transactions.SingleOrDefaultAsync(m => m.Id == id).ConfigureAwait(false);
            databaseContext.Transactions.Remove(transaction);
            await databaseContext.SaveChangesAsync().ConfigureAwait(false);
            return RedirectToAction("TransactionIndex");
        }

        public IActionResult TransactionCreate()
        {
            return View();
        }

        // POST: Transaction/TransactionCreate/{transaction}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TransactionCreate([Bind("Id,TransactionId,TransactionDate,Amount,CurrencyCode,Status")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                databaseContext.Transactions.Add(transaction);
                await databaseContext.SaveChangesAsync().ConfigureAwait(false);
                return RedirectToAction("TransactionIndex");
            }
            return View(transaction);
        }

        // GET: Transaction/TransactionEdit/{id}
        [Route("/TransactionEdit/{id}")]
        public async Task<IActionResult> TransactionEdit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transactions = await databaseContext.Transactions.SingleOrDefaultAsync(m => m.Id == id).ConfigureAwait(false);
            if (transactions == null)
            {
                return NotFound();
            }
            return View(transactions);
        }

        //GET: Transaction/TransactionSearch/{search}/{field}
        [Route("/TransactionSearch/{search}/{field}")]
        public IActionResult TransactionSearch(string search, string field)
        {
            var transactions = from t in databaseContext.Transactions select t;
            switch (field)
            {
                case "TransactionId":
                    transactions = from t in databaseContext.Transactions
                                   where t.TransactionId == search
                                   select t;
                    break;
                case "CurrencyCode":
                    transactions = from t in databaseContext.Transactions
                                   where t.CurrencyCode == search
                                   select t;
                    break;
                default:
                    return NotFound();
            }

            return View(transactions);
        }

        [HttpPost]
        [HttpPost, ActionName("Upload")]
        public ActionResult UploadTransaction(List<IFormFile> files)
        {
            foreach (IFormFile file in files)
            {
                if (file == null || file.Length > 1000000)
                {
                    logger.LogInformation("Request was either Null or File Size was too large. File was: " + file.Length + " Bytes.");
                    return BadRequest();
                }
                var filePath = file.FileName;
                if (Path.GetExtension(filePath) == ".csv")
                {
                    transactionService.UploadCsv(file);
                }
                else if (Path.GetExtension(filePath) == ".xml")
                {
                    transactionService.UploadXml(file);
                }
                else
                {
                    return BadRequest();
                }
            }

            return RedirectToAction("TransactionIndex", "Transaction");
        }
    }
}
