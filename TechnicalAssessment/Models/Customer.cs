using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;
using CsvHelper.Configuration.Attributes;

namespace TechnicalAssessment.Models
{
    [Serializable]
    [XmlRoot("Customers")]
    public class Customer
    {
        [Key]
        [MinLength(10)]
        [MaxLength(10)]
        [Index(0)]
        [XmlElement("Customer Id")]
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "Customer Name is required")]
        [MaxLength(30)]
        [Display(Name = "Customer Name")]
        [Index(1)]
        [XmlElement("Customer Name")]
        public string CustomerName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Please enter a valid email address")]
        [Index(2)]
        [XmlElement("Email")]
        public string Email { get; set; }

        [DataType(DataType.PhoneNumber)]
        [Required(ErrorMessage = "Mobile Number is required.")]
        [Index(3)]
        [XmlElement("Mobile Number")]
        public string MobileNumber { get; set; }

        [Index(4)]
        [XmlArray("Transactions")]
        public ICollection<Transaction> Transactions { get; set; }
    }
}
