using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using CsvHelper;
using log4net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using TechnicalAssessment.Data;
using TechnicalAssessment.Models;
using TechnicalAssessment.Services.Interfaces;

namespace TechnicalAssessment.Services
{
    public class TransactionService : IServiceUpload, IServiceExport<Transaction>
    {
        private DatabaseContext databaseContext;
        private readonly IWebHostEnvironment environment;
        private readonly IFormatProvider formatProvider;
        private readonly ILog logger;

        public TransactionService(DatabaseContext databaseContext, IWebHostEnvironment environment)
        {
            this.databaseContext = databaseContext;
            this.environment = environment;
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
                    var transactions = ParseTransactionXml(nodes);
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

        public List<Transaction> ParseTransactionXml(XmlNodeList xmlNodes)
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

        public string CsvExport(List<Transaction> transactions, string fileName)
        {
            var csv = new StringBuilder();
            if (transactions != null)
            {
                foreach (Transaction transaction in transactions)
                {
                    csv.AppendLine(string.Join(",", transaction));
                }

            }
            return csv.ToString();
        }

        public string XmlExport(List<Transaction> transactions, string fileName)
        {
            string directoryPath = environment.WebRootPath + "\\FileDownloads\\";

            if (Directory.Exists(directoryPath) == false)
            {
                Directory.CreateDirectory(directoryPath);
            }
            string filepath = directoryPath + fileName;
            GZipStream gzipStream = null;
            XmlWriter xmlWriter = null;

            try
            {
                gzipStream = new GZipStream(new FileStream(filepath, FileMode.Create), CompressionMode.Compress);
                XmlWriterSettings xwSettings = new XmlWriterSettings { Encoding = new UTF8Encoding(true) };
                xmlWriter = XmlWriter.Create(gzipStream, xwSettings);
                xmlWriter.WriteStartDocument();
                xmlWriter.WriteStartElement("Transactions");

                foreach (Transaction transaction in transactions)
                {
                    xmlWriter.WriteStartElement("Transaction");
                    xmlWriter.WriteElementString("TransactionId", transaction.TransactionId);
                    xmlWriter.WriteElementString("Amount", transaction.Amount.ToString());
                    xmlWriter.WriteElementString("CurrencyCode", transaction.CurrencyCode);
                    xmlWriter.WriteElementString("TransactionDate", transaction.TransactionDate);
                    xmlWriter.WriteElementString("Status", transaction.Status.ToString());
                    xmlWriter.WriteEndElement();
                }

                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndDocument();
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (xmlWriter != null)
                { 
                    xmlWriter.Flush();
                    xmlWriter.Dispose();
                }
                if (gzipStream != null)
                {
                    gzipStream.Flush();
                    gzipStream.Dispose();
                }
            }

            return filepath;
        }
    }
}
