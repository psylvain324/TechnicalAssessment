using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Xml;
using TechnicalAssessment.Models;
using TechnicalAssessment.Services.Interfaces;

namespace TechnicalAssessment.Services
{
    public class TransactionExportService : IServiceExport<Transaction>
    {
        private readonly IWebHostEnvironment environment;

        public TransactionExportService(IWebHostEnvironment environment)
        {
            this.environment = environment;
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
