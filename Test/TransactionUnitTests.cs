using System;
using System.Globalization;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using OpenQA.Selenium;
using TechnicalAssessment.Controllers;
using TechnicalAssessment.Data;
using TechnicalAssessment.Models;
using TechnicalAssessment.Services.Interfaces;
using Xunit;

namespace Test
{
    public class TransactionUnitTests
    {
        private readonly IWebDriver webDriver;
        private readonly DatabaseContext databaseContext;
        private readonly IServiceUpload<Transaction> transactionServiceUpload;
        private readonly ILogger<TransactionController> logger;

        public TransactionUnitTests(IWebDriver webDriver, DatabaseContext databaseContext, 
            IServiceUpload<Transaction> transactionServiceUpload, ILogger<TransactionController> logger)
        {
            this.webDriver = webDriver;
            this.databaseContext = databaseContext;
            this.transactionServiceUpload = transactionServiceUpload;
            this.logger = logger;
        }

        private Transaction[] GetTestTransactions()
        {
            var testTransaction = new Transaction
            {
                Id = 0,
                TransactionId = "TestInv00001",
                CurrencyCode = "TBH",
                Amount = 100000.00,
                Status = TransactionStatus.Approved,
                TransactionDate = DateTime.Now.ToString(CultureInfo.CurrentCulture),
                CustomerId = 0
            };
            var testTransaction2 = new Transaction
            {
                Id = 1,
                TransactionId = "TestInv00002",
                CurrencyCode = "USD",
                Amount = 500.00,
                Status = TransactionStatus.Finished,
                TransactionDate = DateTime.Now.ToString(CultureInfo.CurrentCulture),
                CustomerId = 0
            };
            var testTransaction3 = new Transaction
            {
                Id = 2,
                TransactionId = "TestInv00003",
                CurrencyCode = "SGD",
                Amount = 750.00,
                Status = TransactionStatus.Rejected,
                TransactionDate = DateTime.Now.ToString(CultureInfo.CurrentCulture),
                CustomerId = 0
            };

            var transactions = new Transaction[]
            {
                    testTransaction,
                    testTransaction2,
                    testTransaction3
            };

            return transactions;
        }

        [Fact]
        //TODO: Finish this Test.
        public void TestCsvUpload()
        {
            var transactions = GetTestTransactions();
            var fileMock = new Mock<IFormFile>();
            var physicalFile = new FileInfo("Test.Resources.TestTransactions.csv");

            var memoryStream = new MemoryStream();
            var streamWriter = new StreamWriter(memoryStream);
            streamWriter.Write(physicalFile.OpenRead());
            streamWriter.Flush();
            memoryStream.Position = 0;

            var fileName = physicalFile.Name;
            fileMock.Setup(mock => mock.FileName).Returns(fileName);
            fileMock.Setup(mock => mock.Length).Returns(memoryStream.Length);
            fileMock.Setup(mock => mock.OpenReadStream()).Returns(memoryStream);
            fileMock.Setup(mock => mock.ContentDisposition).Returns(string.Format("inline; filename={0}", fileName));
            var file = fileMock.Object;

            transactionServiceUpload.UploadCsv(file);
        }
    }
}
