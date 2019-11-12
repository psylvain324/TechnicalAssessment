using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TechnicalAssessment.Models.ViewModels
{
    public class CurrencyViewModel
    {
        [Key]
        public int CurrencyId { get; set; }
        public List<Country> Countries { get; set; }
        public List<Currency> CurrencyCodes { get; set; }
    }
}
