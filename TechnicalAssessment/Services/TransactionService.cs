using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Xml;
using CsvHelper;
using log4net;
using Microsoft.AspNetCore.Http;
using TechnicalAssessment.Data;
using TechnicalAssessment.Models;
using TechnicalAssessment.Services.Interfaces;

namespace TechnicalAssessment.Services
{
    public class TransactionService : IServiceUpload
    {
        private DatabaseContext databaseContext;
        private readonly IFormatProvider formatProvider;
        private readonly ILog logger;


        public TransactionService(DatabaseContext databaseContext)
        {
            this.databaseContext = databaseContext;
            formatProvider = CultureInfo.CreateSpecificCulture(CultureInfo.CurrentCulture.ThreeLetterISOLanguageName);
            logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        }

        public void UploadCsv(IFormFile file)
        {
            if (file != null)
            {
                try
                {
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
                                Amount = double.Parse(csv.GetField("Amount"), formatProvider),
                                CurrencyCode = csv.GetField<string>("CurrencyCode"),
                                TransactionDate = csv.GetField<string>("TransactionDate"),
                                Status = (TransactionStatus)Enum.Parse(typeof(TransactionStatus), csv.GetField("Status"))
                            };

                            databaseContext.Transactions.Add(transaction);
                        }
                    }
                }
                catch(CsvHelperException e)
                {
                    logger.Error(e.InnerException);
                }
            }

            databaseContext.SaveChanges();
        }

        public void UploadXml(IFormFile file)
        {
            XmlDocument doc = new XmlDocument();
            if (file != null)
            {
                try
                {
                    doc.Load(file.OpenReadStream());
                    XmlNodeList nodes = doc.DocumentElement.SelectNodes("/Transactions/Transaction");

                    foreach (XmlNode node in nodes)
                    {
                        Transaction transaction = new Transaction
                        {
                            TransactionId = node.Attributes["id"].Value,
                            TransactionDate = node.SelectSingleNode("TransactionDate").InnerText,
                            CurrencyCode = node.SelectSingleNode("PaymentDetails/CurrencyCode").InnerText,
                            Amount = double.Parse(node.SelectSingleNode("PaymentDetails/Amount").InnerText, formatProvider),
                            Status = (TransactionStatus)Enum.Parse(typeof(TransactionStatus), node.SelectSingleNode("Status").InnerText)
                        };

                        databaseContext.Transactions.Add(transaction);
                    }
                }
                catch(XmlException e)
                {
                    logger.Error(e.InnerException);
                }
            }

            databaseContext.SaveChanges();
        }
    }
}
