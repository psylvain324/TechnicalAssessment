using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TechnicalAssessment.Models.ViewModels
{
    public class CurrencyViewModel
    {
        [Key]
        public int CurrencyId { get; set; }

        public string CountryCode { get; set; }

        public string CurrencyCode { get; set; }

        public Country Country { get; set; }

        public Currency Currency { get; set; }
    }
}
