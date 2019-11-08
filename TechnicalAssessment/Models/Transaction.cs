using System;
using System.ComponentModel.DataAnnotations;
using CsvHelper.Configuration.Attributes;

namespace TechnicalAssessment.Models
{
    public class Transaction
    {
        [Key]
        [MinLength(10)]
        [MaxLength(10)]
        [Index(0)]
        public string TransactionId { get; set; }

        [Required(ErrorMessage = "Amount is required")]
        [Display(Name = "Customer Name")]
        [Index(1)]
        public double Amount { get; set; }

        [Required(ErrorMessage = "Currency Code is required")]
        [DataType(DataType.Currency)]
        [Index(2)]
        public string CurrencyCode { get; set; }

        [DataType(DataType.DateTime)]
        [Required(ErrorMessage = "Transaction Date is required.")]
        [Index(3)]
        public string TransactionDate { get; set; }

        [Index(4)]
        public TransactionStatus Status { get; set; }
    }

    public enum TransactionStatus
    {
        Approved = 0,
        Failed = 1,
    }
}