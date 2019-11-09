using System;
using System.Collections;
using System.Collections.Generic;
using TechnicalAssessment.Models;

namespace TechnicalAssessment.Data
{
    public class CsvUpload
    {
        private void uploadTransaction(string path)
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

        private void uploadCustomer(string path)
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
    }
}
