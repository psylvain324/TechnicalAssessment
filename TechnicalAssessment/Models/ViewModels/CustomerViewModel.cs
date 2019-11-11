using System.Collections.Generic;

namespace TechnicalAssessment.Models.ViewModels
{
    public class CustomerViewModel
    {
        public int CustomerId { get; set; }

        public string CustomerName { get; set; }

        public string Email { get; set; }

        public string MobileNumber { get; set; }

        public List<Transaction> Transactions { get; set; }
    }
}
