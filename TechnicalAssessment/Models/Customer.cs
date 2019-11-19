using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using CsvHelper.Configuration;

namespace TechnicalAssessment.Models
{
    [Serializable]
    [XmlRoot("Customers")]
    public class Customer
    {
        [Key]
        [MaxLength(10)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "Customer Name is required")]
        [MaxLength(30)]
        [Display(Name = "Customer Name")]
        [XmlElement("Customer Name")]
        public string CustomerName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Please enter a valid email address")]
        [XmlElement("Email")]
        public string Email { get; set; }

        [DataType(DataType.PhoneNumber)]
        [Required(ErrorMessage = "Mobile Number is required.")]
        [XmlElement("Mobile Number")]
        public string MobileNumber { get; set; }

        [JsonIgnore]
        public ICollection<Transaction> Transactions { get; set; }
    }

    public sealed class CustomerMap : ClassMap<Customer>
    {
        public CustomerMap()
        {
            Map(m => m.CustomerName).Index(0);
            Map(m => m.Email).Index(1);
            Map(m => m.MobileNumber).Index(2);
        }
    }
}
