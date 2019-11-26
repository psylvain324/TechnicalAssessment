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
    public class TransactionUploadService : IServiceUpload<Transaction>
    {
        private DatabaseContext databaseContext;
        private readonly IFormatProvider formatProvider;
        private readonly ILog logger;

        public TransactionUploadService(DatabaseContext databaseContext)
        {
            this.databaseContext = databaseContext;
            formatProvider = CultureInfo.CreateSpecificCulture(CultureInfo.CurrentCulture.ThreeLetterISOLanguageName);
            logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        }

        public void UploadCsv(IFormFile file)
        {
            if (file != null && file.ContentType.Contains("csv"))
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
            if (file != null && file.ContentType.Equals("xml"))
            {
                try
                {
                    doc.Load(file.OpenReadStream());
                    XmlNodeList nodes = doc.DocumentElement.SelectNodes("/Transactions/Transaction");
                    var transactions = ParseXmlNodes(nodes);
                    foreach (Transaction transaction in transactions)
                    {
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

        public List<Transaction> ParseXmlNodes(XmlNodeList xmlNodes)
        {
            List<Transaction> transactions = new List<Transaction>();
            foreach (XmlNode xmlNode in xmlNodes)
            {
                Transaction transaction = new Transaction
                {
                    TransactionId = xmlNode.Attributes["id"].Value,
                    TransactionDate = xmlNode.SelectSingleNode("TransactionDate").InnerText,
                    CurrencyCode = xmlNode.SelectSingleNode("PaymentDetails/CurrencyCode").InnerText,
                    Amount = double.Parse(xmlNode.SelectSingleNode("PaymentDetails/Amount").InnerText, formatProvider),
                    Status = (TransactionStatus)Enum.Parse(typeof(TransactionStatus), xmlNode.SelectSingleNode("Status").InnerText)
                };

                transactions.Add(transaction);
            }

            return transactions;
        }
    }
}
