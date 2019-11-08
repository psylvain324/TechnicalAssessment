using System;
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
                    TransactionId = int.Parse(data[0]),
                    Amount = double.Parse(data[1]),
                    CurrencyCode = data[2],
                    TransactionDate = data[3],
                    Status = (TransactionStatus)Enum.Parse(typeof(TransactionStatus), data[4])
                };
                dbContext.Transactions.Add(transaction);
            }

            dbContext.SaveChanges();
        }
    }
}
