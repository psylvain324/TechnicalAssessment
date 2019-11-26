using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TechnicalAssessment.Data;
using TechnicalAssessment.Models;
using TechnicalAssessment.Models.ViewModels;

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
        public async Task<IActionResult> CurrencyIndex(string sortOrder, string currentFilter, string searchString, int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["CountrySortParm"] = string.IsNullOrEmpty(sortOrder) ? "country_desc" : "Country";
            ViewData["CurrencySortParm"] = string.IsNullOrEmpty(sortOrder) ? "currency_desc" : "Currency";

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;

            var currencies = from c in databaseContext.Currencies select c;
            if (!string.IsNullOrEmpty(searchString))
            {
                currencies = currencies.Where(s => s.CountryCode.Contains(searchString)|| s.CurrencyCode.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "country_desc":
                    currencies = currencies.OrderByDescending(s => s.CountryCode);
                    break;
                case "Country":
                    currencies = currencies.OrderBy(s => s.CountryCode);
                    break;
                case "currency_desc":
                    currencies = currencies.OrderByDescending(s => s.CurrencyCode);
                    break;
                case "Currency":
                    currencies = currencies.OrderByDescending(s => s.CurrencyCode);
                    break;
                default:
                    currencies = currencies.OrderBy(s => s.CurrencyId);
                    break;
            }

            int pageSize = 10;
            return View(await PaginatedList<Currency>.CreateAsync(currencies.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        //GET: Currency/CurrencyDetails/{name}
        [Route("/CurrencyDetailsByCurrencyCode/{code}")]
        public IActionResult CurrencyDetailsByCurrencyCode(string code)
        {
            var currencies = from c in databaseContext.Currencies select c;
            if (!string.IsNullOrEmpty(code))
            {
                currencies = currencies.Where(s => s.CountryCode.Equals(code));
            }

            if (currencies == null)
            {
                return NotFound();
            }

            return View(currencies);
        }

        //GET: Currency/CurrencyDetails/{id}
        [Route("/CurrencyDetails/{id}")]
        public async Task<IActionResult> CurrencyDetails(int id)
        {
            var currencies = from c in databaseContext.Currencies select c;
            var currency = await currencies.Where(s => s.CurrencyId.Equals(id)).FirstOrDefaultAsync().ConfigureAwait(false);
            return View(currency);
        }

        //GET: Currency/CurrencyDelete/{id}
        public async Task<IActionResult> CurrencyDelete(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            var currency = await databaseContext.Currencies.SingleOrDefaultAsync(m => m.CurrencyId == id).ConfigureAwait(false);
            if (currency == null)
            {
                return NotFound();
            }

            return View(currency);
        }

        //POST: Currency/CurrencyDelete/{id}
        [HttpPost, ActionName("CurrencyDelete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CurrencyConfirmDelete(int id)
        {
            var currency = await databaseContext.Currencies.SingleOrDefaultAsync(m => m.CurrencyId == id).ConfigureAwait(false);
            databaseContext.Currencies.Remove(currency);
            await databaseContext.SaveChangesAsync().ConfigureAwait(false);
            return RedirectToAction("CurrencyIndex");
        }

        public IActionResult CurrencyCreate()
        {
            return View();
        }

        // POST: Currency/CurrencyCreate/{currency}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CurrencyCreate([Bind("CurrencyId,CurrencyCode,CountryCode")] Currency currency)
        {
            if (ModelState.IsValid)
            {
                databaseContext.Currencies.Add(currency);
                await databaseContext.SaveChangesAsync().ConfigureAwait(false);
                return RedirectToAction("CurrencyIndex");
            }
            return View(currency);
        }

        // GET: Currency/CurrencyEdit/{id}
        [Route("/CurrenctEdit/{id}")]
        public async Task<IActionResult> CurrencyEdit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var currencies = await databaseContext.Currencies.SingleOrDefaultAsync(m => m.CurrencyId == id).ConfigureAwait(false);
            if (currencies == null)
            {
                return NotFound();
            }
            return View(currencies);
        }
    }
}
