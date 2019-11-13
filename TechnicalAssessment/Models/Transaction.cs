using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;
using System.Text.Json.Serialization;

namespace TechnicalAssessment.Models
{
    [Serializable]
    [XmlRoot("Transactions")]
    public class Transaction
    {
        [Key]
        public int id { get; set; }

        [MaxLength(50)]
        [Required(ErrorMessage = "Amount is required")]
        [XmlElement("Transaction Id")]
        [CsvHelper.Configuration.Attributes.Index(0)]
        public string TransactionId { get; set; }

        [Required(ErrorMessage = "Amount is required")]
        [Display(Name = "Customer Name")]
        [XmlElement("Amount")]
        [CsvHelper.Configuration.Attributes.Index(1)]
        public double Amount { get; set; }

        [Required(ErrorMessage = "Currency Code is required")]
        [DataType(DataType.Currency)]
        [XmlElement("Currency Code")]
        [CsvHelper.Configuration.Attributes.Index(2)]
        public string CurrencyCode { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}{1:HH/mm}")]
        [Required(ErrorMessage = "Transaction Date is required.")]
        [XmlElement("Transaction Date")]
        [CsvHelper.Configuration.Attributes.Index(3)]
        public string TransactionDate { get; set; }

        [Required(ErrorMessage = "Transaction Status is required.")]
        [XmlElement("Status")]
        [CsvHelper.Configuration.Attributes.Index(4)]
        public TransactionStatus Status { get; set; }

        [ForeignKey("CustomerId")]
        public int CustomerId { get; set; }

        public Customer Customer { get; set; }
    }

    public enum TransactionStatus
    {
        Approved = 0,
        Failed = 1,
        Finished = 2
    }
}