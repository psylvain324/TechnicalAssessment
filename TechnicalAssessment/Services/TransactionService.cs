using System;
using System.Collections.Generic;
using System.Xml;
using TechnicalAssessment.Data;
using TechnicalAssessment.Models;
using TechnicalAssessment.Services.Interfaces;

namespace TechnicalAssessment.Services
{
    public class TransactionService : IServiceUpload
    {
        private DatabaseContext databaseContext;

        public TransactionService(DatabaseContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }

        public void UploadCsv(string path)
        {
            using var streamReader = System.IO.File.OpenText(path);
            while (!streamReader.EndOfStream)
            {
                var line = streamReader.ReadLine();
                var data = line.Split(new[] { ',' });
                var transaction = new Transaction()
                {
                    TransactionId = data[0],
                    Amount = double.Parse(data[1]),
                    CurrencyCode = data[2],
                    TransactionDate = data[3],
                    Status = (TransactionStatus)Enum.Parse(typeof(TransactionStatus), data[4])
                };
                databaseContext.Transactions.Add(transaction);
            }

            databaseContext.SaveChanges();
        }

        public void UploadXml(string filePath)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(filePath);

            XmlNodeList nodes = doc.DocumentElement.SelectNodes("/Transactions/Transaction");
            List<Transaction> transactions = new List<Transaction>();
            foreach (XmlNode node in nodes)
            {
                Transaction transaction = new Transaction
                {
                    TransactionId = node.Attributes["Transaction Id"].Value,
                    CurrencyCode = node.SelectSingleNode("Currency Code").InnerText,
                    TransactionDate = node.SelectSingleNode("Transaction Date").InnerText,
                    Amount = double.Parse(node.SelectSingleNode("Amount").InnerText),
                    Status = (TransactionStatus)Enum.Parse(typeof(TransactionStatus), node.SelectSingleNode("status").InnerText)
                };

                transactions.Add(transaction);
                databaseContext.Transactions.Add(transaction);
            }

            databaseContext.SaveChanges();
        }
    }
}
