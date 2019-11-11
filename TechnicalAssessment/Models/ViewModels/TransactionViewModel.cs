using TechnicalAssessment.Models;

namespace TechnicalAssessment.Data
{
    public class TransactionViewModel
    {
        public string TransactionId { get; set; }

        public double Amount { get; set; }

        public string CurrencyCode { get; set; }

        public string TransactionDate { get; set; }

        public TransactionStatus Status { get; set; }
    }
}
