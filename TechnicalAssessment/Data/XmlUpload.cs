using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
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
                    TransactionId = node.Attributes["id"].Value,
                    CurrencyCode = node.SelectSingleNode("currencycode").InnerText,
                    TransactionDate = node.SelectSingleNode("transactiondate").InnerText,
                    Amount = Double.Parse(node.SelectSingleNode("amount").InnerText),
                    Status = (TransactionStatus)Enum.Parse(typeof(TransactionStatus), node.SelectSingleNode("status").InnerText)
                };

                transactions.Add(transaction);
            }
        }
    }
}
