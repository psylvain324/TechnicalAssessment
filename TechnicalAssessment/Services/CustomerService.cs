using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using TechnicalAssessment.Data;
using TechnicalAssessment.Models;

namespace TechnicalAssessment.Services
{
    public class CustomerService
    {
        private const string success = "File uploaded successfully";

        public string UploadTransaction(string path)
        {
            string extensionType = Path.GetExtension(path);
            try
            {
                if (extensionType == "csv")
                {
                    UploadCustomerCsv(path);
                }
                else if (extensionType == "xml")
                {
                    ParseCustomerXml(path);
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }

            return success;
        }

        public void UploadCustomerCsv(string path)
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
                List<Transaction> transactions = new List<Transaction>();
                transactions.Add(transaction);
                var customer = new Customer()
                {
                    CustomerId = int.Parse(data[0]),
                    CustomerName = data[1],
                    Email = data[2],
                    MobileNumber = data[3],
                    Transactions = transactions
                };
                dbContext.Transactions.Add(transaction);
                dbContext.Customers.Add(customer);
            }

            dbContext.SaveChanges();
        }

        public void ParseCustomerXml(string filePath)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(filePath);

            XmlNodeList nodes = doc.DocumentElement.SelectNodes("/Customers/Customer");
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
