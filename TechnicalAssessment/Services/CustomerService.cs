using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using TechnicalAssessment.Data;
using TechnicalAssessment.Models;
using TechnicalAssessment.Services.Interfaces;

namespace TechnicalAssessment.Services
{
    public class CustomerService : IServiceUpload
    {
        private DatabaseContext databaseContext;

        public CustomerService(DatabaseContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }

        public void UploadCsv(string filePath)
        {
            using var streamReader = File.OpenText(filePath);
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
                databaseContext.Transactions.Add(transaction);
                databaseContext.Customers.Add(customer);
            }

            databaseContext.SaveChanges();
        }

        public void UploadXml(string filePath)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(filePath);

            XmlNodeList nodes = doc.DocumentElement.SelectNodes("/Customers/Customer");
            foreach (XmlNode node in nodes)
            {
                var transactions = node.SelectNodes("/Transactions");
                Customer customer = new Customer
                {
                    CustomerId = int.Parse(node.Attributes["Customer Id"].Value),
                    CustomerName = node.SelectSingleNode("Customer Name").InnerText,
                    Email = node.SelectSingleNode("Email").InnerText,
                    MobileNumber = node.SelectSingleNode("Mobile Number").InnerText,
                    Transactions = (ICollection<Transaction>)transactions
                };
                foreach(Transaction transaction in transactions)
                {
                    databaseContext.Transactions.Add(transaction);
                }
                databaseContext.Customers.Add(customer);
            }

        }
    }
}
