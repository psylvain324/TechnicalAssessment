using System;
using System.Xml.Serialization;
using CsvHelper.Configuration.Attributes;

namespace TechnicalAssessment.Models.ViewModels
{
    [Serializable]
    [XmlRoot("Transactions")]
    public class TransactionViewModel
    {
        [XmlElement("Transaction Id")]
        [Index(0)]
        public string TransactionId { get; set; }

        [XmlElement("Amount")]
        [Index(1)]
        public double Amount { get; set; }

        [XmlElement("Currency Code")]
        [Index(2)]
        public string CurrencyCode { get; set; }

        [XmlElement("Transaction Date")]
        [Index(3)]
        public string TransactionDate { get; set; }

        [XmlElement("Status")]
        [Index(4)]
        public TransactionStatus Status { get; set; }
    }
}