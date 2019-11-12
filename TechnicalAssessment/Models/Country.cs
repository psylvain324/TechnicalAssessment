using System.ComponentModel.DataAnnotations;

namespace TechnicalAssessment.Models.ViewModels
{
    public class Country
    {
        [Key]
        public int CountryId { get; set; }
        public string CountryCode { get; set; }
    }
}
