using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TechnicalAssessment.Models.ViewModels
{
    public class CurrencyViewModel
    {
        [Key]
        public int CurrencyId { get; set; }

        public Country Country { get; set; }

        public Currency Currency { get; set; }

        [JsonIgnore]
        public List<Country> Countries { get; set; }

        [JsonIgnore]
        public List<Currency> CurrencyCodes { get; set; }
    }
}
