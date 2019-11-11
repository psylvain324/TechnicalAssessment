using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TechnicalAssessment.Models;
using TechnicalAssessment.Models.ViewModels;

namespace TechnicalAssessment.Data
{
    public class DataGenerator
    {
        static List<string> GetCountryCodes()
        {
            List<string> countryCodes = CultureInfo
                .GetCultures(CultureTypes.AllCultures)
                .Where(c => !c.IsNeutralCulture)
                .Where(ri => ri != null)
                .Select(ri => ri.DisplayName)
                .ToList();
            return countryCodes;
        }

        static List<string> GetCurrencyCodes()
        {
            List<string> codes = new List<string>();
            var cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
            foreach (var culture in cultures)
            {
                var region = new RegionInfo(culture.Name);
                codes.Add(region.ISOCurrencySymbol);
            }
            return codes;
        }

        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new DatabaseContext(
                serviceProvider.GetRequiredService<DbContextOptions<DatabaseContext>>()))
            {
                context.Database.EnsureCreated();
                if (context.Transactions.Any())
                {
                    return;
                }
                var testTransaction = new Transaction
                {
                    TransactionId = "Inv00001",
                    CurrencyCode = "TBH",
                    Amount = 100000.00,
                    Status = TransactionStatus.Approved,
                    TransactionDate = DateTime.Now.ToString()
                };

                var transactions = new Transaction[]
                {
                testTransaction
                };

                var testCustomer = new Customer
                {
                    CustomerId = 00001,
                    CustomerName = "Phillip Sylvain",
                    Email = "psylvain324@gmail.com",
                    MobileNumber = "16032862905",
                    Transactions = transactions
                };

                var currencies = new CurrencyViewModel
                {
                    Countries = GetCountryCodes(),
                    CurrencyCodes = GetCurrencyCodes()
                };

                context.Transactions.Add(testTransaction);
                context.Customers.Add(testCustomer);
                context.SaveChanges();
            }
        }
    }
}