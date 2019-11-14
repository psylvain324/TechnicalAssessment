using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TechnicalAssessment.Data;


namespace TechnicalAssessment.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ILogger<TransactionController> logger;
        private DatabaseContext databaseContext;

        public CustomerController(ILogger<TransactionController> logger, DatabaseContext databaseContext)
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
            return View(await databaseContext.Customers.ToListAsync());
        }

        [Route("{id}")]
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await databaseContext.Customers.FirstOrDefaultAsync();
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        [Route("{search}")]
        public IActionResult Details(string search, string field)
        {
            var customers = from c in databaseContext.Customers select c;
            switch (field)
            {
                case "CustomerID":
                    customers = from c in databaseContext.Customers
                                   where c.CustomerId == int.Parse(search)
                                   select c;
                    break;
                case "Email":
                    customers = from c in databaseContext.Customers
                                   where c.Email == search
                                   select c;
                    break;
                default:
                    return NotFound();
            }

            return View(customers);
        }
    }
}
