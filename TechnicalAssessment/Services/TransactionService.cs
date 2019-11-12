using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using TechnicalAssessment.Models;

namespace TechnicalAssessment.Data
{
    public class TransactionService
    {
        private const string success = "File uploaded successfully";

        public string UploadTransaction(string path)
        {
            string extensionType = Path.GetExtension(path);
            try
            {
                if (extensionType == "csv")
                {
                    UploadTransactionCsv(path);
                }
                else if (extensionType == "xml")
                {
                    ParseTransactionXml(path);
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }

            return success;
        }

        public void UploadTransactionCsv(string path)
        {
            using var streamReader = System.IO.File.OpenText(path);
            var dbContext = new DatabaseContext(new Microsoft.EntityFrameworkCore.DbContextOptions<DatabaseContext>());
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
                dbContext.Transactions.Add(transaction);
            }

            dbContext.SaveChanges();
        }

        public void ParseTransactionXml(string filePath)
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
                    TransactionDate = node.SelectSingleNode("transactiondate").InnerText,
                    Amount = double.Parse(node.SelectSingleNode("Amount").InnerText),
                    Status = (TransactionStatus)Enum.Parse(typeof(TransactionStatus), node.SelectSingleNode("status").InnerText)
                };

                transactions.Add(transaction);
            }
        }
    }
}
