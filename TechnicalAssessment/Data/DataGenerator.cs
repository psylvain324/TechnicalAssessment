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
        public static void Initialize(DatabaseContext databaseContext)
        {
            databaseContext.Database.EnsureCreated();
            if (databaseContext.Transactions.Any())
            {
                return;
            }
            var testTransaction = new Transaction
            {
                TransactionId = "Inv00001",
                CurrencyCode = "TBH",
                Amount = 100000.00,
                Status = TransactionStatus.Approved,
                TransactionDate = DateTime.Now.ToString(),
                CustomerId = 0
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

            databaseContext.Transactions.Add(testTransaction);
            databaseContext.Customers.Add(testCustomer);
            databaseContext.SaveChanges();
        }

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
                    if (IsValidRegion(culture.TwoLetterISOLanguageName, out bool isValid))
                    {
                        var region = new RegionInfo(culture.ThreeLetterISOLanguageName);
                        currencyCodes.Add(region.ISOCurrencySymbol);
                    }
                }
                catch (Exception)
                {
                    throw new Exception();
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
        public static bool IsValidRegion(string isoCountryCode, out bool isValid)
        {
            return isValid = CultureInfo.GetCultures(CultureTypes.AllCultures)
                .Where(x => !x.Equals(CultureInfo.InvariantCulture))
                .Where(x => !x.IsNeutralCulture)
                .Select(x => new RegionInfo(x.LCID))
                .Any(x => x.Name.Equals(isoCountryCode, StringComparison.InvariantCulture));
        }
    }
}