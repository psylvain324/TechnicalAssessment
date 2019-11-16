using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TechnicalAssessment.Data;

namespace TechnicalAssessment.Controllers
{
    public class CurrencyController : Controller
    {
        private readonly ILogger<TransactionController> logger;
        private readonly DatabaseContext databaseContext;

        public CurrencyController(ILogger<TransactionController> logger, DatabaseContext databaseContext)
        {
            this.logger = logger;
            this.databaseContext = databaseContext;
        }
        // GET: Currency
        public async Task<IActionResult> CurrencyIndex()
        {
            return View(await databaseContext.Currencies.ToListAsync().ConfigureAwait(false));
        }
    }
}
