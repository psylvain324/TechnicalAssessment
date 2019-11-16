using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
                TransactionDate = DateTime.Now.ToString(CultureInfo.CurrentCulture),
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
            var currencies = GetCurrencyViewModels();
            foreach(CurrencyViewModel currencyViewModel in currencies)
            {
                databaseContext.Add(currencyViewModel);
            }
            databaseContext.Transactions.Add(testTransaction);
            databaseContext.Customers.Add(testCustomer);
            databaseContext.SaveChanges();
        }

        public static List<CurrencyViewModel> GetCurrencyViewModels()
        {
            List<CurrencyViewModel> currencyViewModels = new List<CurrencyViewModel>();

            var currencyDictionary = CultureInfo
                .GetCultures(CultureTypes.SpecificCultures)
                .Where(ri => ri != null)
                .Distinct()
                .ToDictionary(ri => ri.EnglishName,
                                (ri => ri.NumberFormat.CurrencySymbol));
            foreach (KeyValuePair<string, string> entry in currencyDictionary)
            {
                int index = 0;
                Country country = new Country
                {
                    CountryId = index,
                    CountryCode = entry.Key
                };
                Currency currency = new Currency
                {
                    CurrencyId = index,
                    CurrencyCode = entry.Value
                };
                currencyViewModels.Add(new CurrencyViewModel
                {
                    CurrencyId = index,
                    Country = country,
                    Currency = currency
                });

                index++;
            }
            return currencyViewModels;
        }

        public static List<Country> GetCountryCodes()
        {
            List<Country> countries = new List<Country>();
            List<Currency> currencies = new List<Currency>();

            List<string> countryCodes = CultureInfo
                .GetCultures(CultureTypes.AllCultures)
                .Where(c => !c.IsNeutralCulture)
                .Where(ri => ri != null)
                .Select(ri => ri.ThreeLetterISOLanguageName)
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

        public static List<Currency> GetCurrencyCodes()
        {
            List<Currency> currencies = new List<Currency>();
            List<string> currencyCodes = CultureInfo
                .GetCultures(CultureTypes.AllCultures)
                .Where(c => !c.IsNeutralCulture)
                .Where(ri => ri != null)
                .Select(ri => ri.NumberFormat.CurrencySymbol)
                .ToList();
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