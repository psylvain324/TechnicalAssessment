using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using CsvHelper;
using CsvHelper.TypeConversion;
using Microsoft.AspNetCore.Http;
using TechnicalAssessment.Data;
using TechnicalAssessment.Models;
using TechnicalAssessment.Services.Interfaces;
using TinyCsvParser;

namespace TechnicalAssessment.Services
{
    public class TransactionService : IServiceUpload
    {
        private DatabaseContext databaseContext;

        public TransactionService(DatabaseContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }

        public void UploadCsv(IFormFile file)
        {
            IFormatProvider provider = CultureInfo.CreateSpecificCulture(CultureInfo.CurrentCulture.ThreeLetterISOLanguageName);
            using (var reader = new StreamReader(file.OpenReadStream()))
            using (var csv = new CsvReader(reader))
            {
                var transactions = new List<Transaction>();
                csv.Read();
                csv.ReadHeader();
                while (csv.Read())
                {
                    var transaction = new Transaction
                    {
                        TransactionId = csv.GetField<string>("TransactionId"),
                        Amount = double.Parse(csv.GetField("Amount"), provider),
                        CurrencyCode = csv.GetField<string>("CurrencyCode"),
                        TransactionDate = csv.GetField<string>("TransactionDate"),
                        Status = (TransactionStatus)Enum.Parse(typeof(TransactionStatus), csv.GetField("Status"))
                    };

                    databaseContext.Transactions.Add(transaction);
                }
            }
            /* TinyCsvParser
            CsvParserOptions csvParserOptions = new CsvParserOptions(true, ',');
            var csvParser = new CsvParser<Transaction>(csvParserOptions, new CsvTransactionMapping());
            var records = csvParser.ReadFromFile(file.FileName, Encoding.UTF8);
            var transactions = records.Select(x => x.Result).ToList();

            foreach (var transaction in transactions)
            {
                databaseContext.Transactions.Add(transaction);
            }
            */
            databaseContext.SaveChanges();
        }

        public void UploadXml(IFormFile file)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(file.OpenReadStream());
            XmlNodeList nodes = doc.DocumentElement.SelectNodes("/Transactions/Transaction");

            foreach (XmlNode node in nodes)
            {
                Transaction transaction = new Transaction
                {
                    TransactionId = node.Attributes["id"].Value,
                    TransactionDate = node.SelectSingleNode("TransactionDate").InnerText,
                    CurrencyCode = node.SelectSingleNode("PaymentDetails/CurrencyCode").InnerText,
                    Amount = double.Parse(node.SelectSingleNode("PaymentDetails/Amount").InnerText),
                    Status = (TransactionStatus)Enum.Parse(typeof(TransactionStatus), node.SelectSingleNode("Status").InnerText)
                };

                databaseContext.Transactions.Add(transaction);
            }

            databaseContext.SaveChanges();
        }
    }
}
