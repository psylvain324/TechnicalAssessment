﻿using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TechnicalAssessment.Data;


namespace TechnicalAssessment.Controllers
{
    [Route("Customer")]
    public class CustomerController : Controller
    {
        private readonly ILogger<TransactionController> logger;
        private readonly DatabaseContext databaseContext;

        public CustomerController(ILogger<TransactionController> logger, DatabaseContext databaseContext)
        {
            this.logger = logger;
            this.databaseContext = databaseContext;
        }

        //GET: Customer
        public async Task<IActionResult> Index()
        {
            return View(await databaseContext.Customers.ToListAsync());
        }

        //GET: Customer/Details/{id}
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

        // GET: Customer/Edit/{id}
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customers = await databaseContext.Customers.SingleOrDefaultAsync(m => m.CustomerId == id);
            if (customers == null)
            {
                return NotFound();
            }
            return View(customers);
        }

        //GET Customer/Search/{search}/{field}
        [Route("{search}")]
        public IActionResult Search(string search, string field)
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
