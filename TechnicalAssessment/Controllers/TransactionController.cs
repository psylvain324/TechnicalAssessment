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
        private DatabaseContext databaseContext;

        public TransactionController(ILogger<TransactionController> logger, DatabaseContext databaseContext)
        {
            this.logger = logger;
            this.databaseContext = databaseContext;
        }

        public IActionResult Index()
        {
            return View();
        }
  
        public async Task<IActionResult> Details()
        {
            return View(await databaseContext.Transactions.ToListAsync());
        }

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

        [Route("{search}")]
        public IActionResult Details(string search, string field)
        {
            var transactions = from t in databaseContext.Transactions select t;
            switch (field)
            {
                case "TransactionId":
                    transactions = from t in databaseContext.Transactions
                                   where t.CurrencyCode == search
                                   select t;
                    break;
                case "CurrencyCode":
                    transactions = from t in databaseContext.Transactions
                                   where t.TransactionId == search
                                   select t;
                    break;
                default:
                    return NotFound();
            }

            return View(transactions);
        }
    }
}
