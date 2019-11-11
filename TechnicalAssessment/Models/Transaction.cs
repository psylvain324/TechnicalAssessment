using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TechnicalAssessment.Models
{
    public class Transaction
    {
        [Key]
        [MinLength(10)]
        [MaxLength(10)]
        [CsvHelper.Configuration.Attributes.Index(0)]
        public string TransactionId { get; set; }

        [Required(ErrorMessage = "Amount is required")]
        [Display(Name = "Customer Name")]
        [CsvHelper.Configuration.Attributes.Index(1)]
        public double Amount { get; set; }

        [Required(ErrorMessage = "Currency Code is required")]
        [DataType(DataType.Currency)]
        [CsvHelper.Configuration.Attributes.Index(2)]
        public string CurrencyCode { get; set; }

        [DataType(DataType.DateTime)]
        [Required(ErrorMessage = "Transaction Date is required.")]
        [CsvHelper.Configuration.Attributes.Index(3)]
        public string TransactionDate { get; set; }

        [Required(ErrorMessage = "Transaction Status is required.")]
        [CsvHelper.Configuration.Attributes.Index(4)]
        public TransactionStatus Status { get; set; }

        [Required]
        [ForeignKey("CustomerId")]
        [CsvHelper.Configuration.Attributes.Index(5)]
        public Customer CustomerTransaction { get; set; }
    }

    public enum TransactionStatus
    {
        Approved = 0,
        Failed = 1,
        Finished = 2
    }
}