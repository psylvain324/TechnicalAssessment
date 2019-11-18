using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using CsvHelper.Configuration.Attributes;

namespace TechnicalAssessment.Models
{
    [Serializable]
    [XmlRoot("Customers")]
    public class Customer
    {
        [Key]
        [MaxLength(10)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [CsvHelper.Configuration.Attributes.Index(0)]
        [XmlElement("Customer Id")]
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "Customer Name is required")]
        [MaxLength(30)]
        [Display(Name = "Customer Name")]
        [CsvHelper.Configuration.Attributes.Index(1)]
        [XmlElement("Customer Name")]
        public string CustomerName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Please enter a valid email address")]
        [CsvHelper.Configuration.Attributes.Index(2)]
        [XmlElement("Email")]
        public string Email { get; set; }

        [DataType(DataType.PhoneNumber)]
        [Required(ErrorMessage = "Mobile Number is required.")]
        [CsvHelper.Configuration.Attributes.Index(3)]
        [XmlElement("Mobile Number")]
        public string MobileNumber { get; set; }

        [JsonIgnore]
        public ICollection<Transaction> Transactions { get; set; }
    }
}
