using System.ComponentModel.DataAnnotations;

namespace TechnicalAssessment.Models.ViewModels
{
    public class CurrencyViewModel
    {
        [Key]
        public int CurrencyId { get; set; }

        public Country Country { get; set; }

        public Currency Currency { get; set; }
    }
}
