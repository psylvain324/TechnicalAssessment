using System;
using System.Collections.Generic;
using System.Xml;
using TechnicalAssessment.Models;

namespace TechnicalAssessment.Data
{
    public class XmlUpload
    {
        public void ParseTransactionXML(string filePath)
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
                    Amount = Double.Parse(node.SelectSingleNode("Amount").InnerText),
                    Status = (TransactionStatus)Enum.Parse(typeof(TransactionStatus), node.SelectSingleNode("status").InnerText)
                };

                transactions.Add(transaction);
            }
        }
    }
}
