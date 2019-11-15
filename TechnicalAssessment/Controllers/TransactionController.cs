using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TechnicalAssessment.Data;

namespace TechnicalAssessment.Controllers
{
    public class TransactionController : Controller
    {
        private readonly ILogger<TransactionController> logger;
        private readonly DatabaseContext databaseContext;

        public TransactionController(ILogger<TransactionController> logger, DatabaseContext databaseContext)
        {
            this.logger = logger;
            this.databaseContext = databaseContext;
        }

        //GET: Transaction
        public async Task<IActionResult> Index()
        {
            return View(await databaseContext.Transactions.ToListAsync());
        }

        //GET: Transaction/Details/{id}
        [Route("/Details/{id}")]
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transaction = await databaseContext.Transactions.FirstOrDefaultAsync();
            if (transaction == null)
            {
                return NotFound();
            }

            return View(transaction);
        }

        // GET: Transaction/Edit/{id}
        [Route("/Edit/{id}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transactions = await databaseContext.Transactions.SingleOrDefaultAsync(m => m.Id == id);
            if (transactions == null)
            {
                return NotFound();
            }
            return View(transactions);
        }

        //GET: Transaction/Search/{search}/{field}
        [Route("/Search/{search}/{field}")]
        public IActionResult Search(string search, string field)
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
    }
}
