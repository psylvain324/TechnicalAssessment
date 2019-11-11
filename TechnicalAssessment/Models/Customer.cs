using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CsvHelper.Configuration.Attributes;

namespace TechnicalAssessment.Models
{
    public class Customer
    {
        [Key]
        [MinLength(10)]
        [MaxLength(10)]
        [Index(0)]
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "Customer Name is required")]
        [MaxLength(30)]
        [Display(Name = "Customer Name")]
        [Index(1)]
        public string CustomerName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Please enter a valid email address")]
        [Index(2)]
        public string Email { get; set; }

        [DataType(DataType.PhoneNumber)]
        [Required(ErrorMessage = "Mobile Number is required.")]
        [Index(3)]
        public string MobileNumber { get; set; }

        [Required]
        [Index(4)]
        public ICollection<Transaction> Transactions { get; set; }
    }
}
