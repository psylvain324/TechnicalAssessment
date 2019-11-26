using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TechnicalAssessment.Models;

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
                Id = 0,
                TransactionId = "Inv00001",
                CurrencyCode = "TBH",
                Amount = 100000.00,
                Status = TransactionStatus.Approved,
                TransactionDate = DateTime.Now.ToString(CultureInfo.CurrentCulture),
                CustomerId = 0
            };
            var testTransaction2 = new Transaction
            {
                Id = 2,
                TransactionId = "Inv00009",
                CurrencyCode = "USD",
                Amount = 500.00,
                Status = TransactionStatus.Finished,
                TransactionDate = DateTime.Now.ToString(CultureInfo.CurrentCulture),
                CustomerId = 0
            };
            var testTransaction3 = new Transaction
            {
                Id = 3,
                TransactionId = "Inv00002",
                CurrencyCode = "SGD",
                Amount = 750.00,
                Status = TransactionStatus.Rejected,
                TransactionDate = DateTime.Now.ToString(CultureInfo.CurrentCulture),
                CustomerId = 0
            };
            var testTransaction4 = new Transaction
            {
                Id = 4,
                TransactionId = "Inv00003",
                CurrencyCode = "PHP",
                Amount = 5000.00,
                Status = TransactionStatus.Finished,
                TransactionDate = DateTime.Now.ToString(CultureInfo.CurrentCulture),
                CustomerId = 0
            };
            var testTransaction5 = new Transaction
            {
                Id = 5,
                TransactionId = "Inv00004",
                CurrencyCode = "VND",
                Amount = 550000.00,
                Status = TransactionStatus.Failed,
                TransactionDate = DateTime.Now.ToString(CultureInfo.CurrentCulture),
                CustomerId = 0
            };
            var testTransaction6 = new Transaction
            {
                Id = 6,
                TransactionId = "Inv00005",
                CurrencyCode = "IDR",
                Amount = 650000.00,
                Status = TransactionStatus.Approved,
                TransactionDate = DateTime.Now.ToString(CultureInfo.CurrentCulture),
                CustomerId = 0
            };

            var transactions = new Transaction[]
            {
                    testTransaction,
                    testTransaction2,
                    testTransaction3,
                    testTransaction4,
                    testTransaction5,
                    testTransaction6
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
            foreach(Currency currency in currencies)
            {
                databaseContext.Add(currency);
            }
            foreach(Transaction transaction in transactions)
            {
                databaseContext.Transactions.Add(transaction);
            }
            databaseContext.Customers.Add(testCustomer);
            databaseContext.SaveChanges();
        }

        public static List<Currency> GetCurrencyViewModels()
        {
            List<Currency> currencies = new List<Currency>();

            var cultures = CultureInfo
                .GetCultures(CultureTypes.SpecificCultures)
                .Where(ri => ri != null)
                .Select(ri => ri).ToList();

            var distinctCultures = cultures.GroupBy(x => x.LCID).Select(y => y.First()).ToList();

            for (int i = 1; i < distinctCultures.Count; i++)
            {
                if (!distinctCultures[i].Equals(CultureInfo.InvariantCulture))
                {
                    var regionCulture = new RegionInfo(distinctCultures[i].LCID);
                    Currency currency = new Currency
                    {
                        CurrencyId = i,
                        CurrencyCode = regionCulture.ISOCurrencySymbol,
                        CountryCode = regionCulture.EnglishName
                    };
                    currencies.Add(currency);
                }
            }
            return currencies;
        }

        public static List<string> GetCountryCodes()
        {
            List<string> countries = new List<string>();
            List<Currency> currencies = new List<Currency>();

            List<string> countryCodes = CultureInfo
                .GetCultures(CultureTypes.AllCultures)
                .Where(c => !c.IsNeutralCulture)
                .Where(ri => ri != null)
                .Select(ri => ri.ThreeLetterISOLanguageName)
                .ToList();
            for (int i = 0; i < countryCodes.Count; i++)
            {
                countries.Add(countryCodes[i]);

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