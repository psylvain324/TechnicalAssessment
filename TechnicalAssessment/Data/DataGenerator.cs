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
    public static class DataGenerator
    {
        static List<Country> GetCountryCodes()
        {
            List<Country> countries = new List<Country>();
            List<string> countryCodes = CultureInfo
                .GetCultures(CultureTypes.AllCultures)
                .Where(c => !c.IsNeutralCulture)
                .Where(ri => ri != null)
                .Select(ri => ri.DisplayName)
                .ToList();
            for (int i = 0; i < countryCodes.Count; i++)
            {
                Country country = new Country
                {
                    CountryId = i,
                    CountryCode = countryCodes[i]
                };
                countries.Add(country);
            }
            return countries;
        }

        static List<Currency> GetCurrencyCodes()
        {
            List<Currency> currencies = new List<Currency>();
            List<string> currencyCodes = new List<string>();
            var cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
            foreach (var culture in cultures)
            {
                try
                {
                    var region = new RegionInfo(culture.Name);
                    currencyCodes.Add(region.ISOCurrencySymbol);
                }
                catch(Exception)
                {
                    continue;
                }
            }
            for (int i = 0; i < currencyCodes.Count; i++)
            {
                Currency currency = new Currency
                {
                    CurrencyId = i,
                    CurrencyCode = currencyCodes[i]
                };
                currencies.Add(currency);
            }
            return currencies;
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
                    CustomerId = 0,
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