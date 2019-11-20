using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TechnicalAssessment.Data;
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
            return View(await PaginatedList<CurrencyViewModel>.CreateAsync(currencies.AsNoTracking(), pageNumber ?? 1, pageSize));
        }
    }
}
