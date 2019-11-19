using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;
using System.Text.Json.Serialization;
using CsvHelper.Configuration;

namespace TechnicalAssessment.Models
{
    [Serializable]
    [XmlRoot("Transactions")]
    public class Transaction
    {
        [Key]
        [MaxLength(10)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(50)]
        [Required(ErrorMessage = "Amount is required")]
        [XmlElement("Transaction Id")]
        public string TransactionId { get; set; }

        [Required(ErrorMessage = "Amount is required")]
        [XmlElement("Amount")]
        public double Amount { get; set; }

        [Required(ErrorMessage = "Currency Code is required")]
        [XmlElement("Currency Code")]
        public string CurrencyCode { get; set; }

        [DataType(DataType.DateTime)]
        [Required(ErrorMessage = "Transaction Date is required.")]
        [XmlElement("Transaction Date")]
        public string TransactionDate { get; set; }

        [Required(ErrorMessage = "Transaction Status is required.")]
        [XmlElement("Status")]
        public TransactionStatus Status { get; set; }

        [ForeignKey("CustomerId")]
        [JsonIgnore]
        public int? CustomerId { get; set; }

        public Customer Customer { get; set; }
    }

    public sealed class TransactionMap : ClassMap<Transaction>
    {
        public TransactionMap()
        {
            Map(m => m.TransactionId).Index(0);
            Map(m => m.Amount).Index(1);
            Map(m => m.CurrencyCode).Index(2);
            Map(m => m.TransactionDate).Index(3);
            Map(m => m.Status).Index(4);
        }
    }

    public enum TransactionStatus
    {
        Approved = 0,
        Failed = 1,
        Rejected = 2,
        Finished = 3
    }
}