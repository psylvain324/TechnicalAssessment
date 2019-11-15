using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TechnicalAssessment.Data;

namespace TechnicalAssessment.Controllers
{
    [Route("Transactions")]
    public class TransactionController : Controller
    {
        private readonly ILogger<TransactionController> logger;
        private DatabaseContext databaseContext;

        public TransactionController(ILogger<TransactionController> logger, DatabaseContext databaseContext)
        {
            this.logger = logger;
            this.databaseContext = databaseContext;
        }

        //GET: Transactions
        public async Task<IActionResult> Index()
        {
            return View(await databaseContext.Transactions.ToListAsync());
        }

        //GET: Transactions/Details/{id}
        [Route("{id}")]
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

        // GET: Transactions/Edit/{id}
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transactions = await databaseContext.Transactions.SingleOrDefaultAsync(m => m.id == id);
            if (transactions == null)
            {
                return NotFound();
            }
            return View(transactions);
        }

        //GET: Transactions/Details/{search}/{field}
        [Route("{search}/{field}")]
        public IActionResult Details(string search, string field)
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
