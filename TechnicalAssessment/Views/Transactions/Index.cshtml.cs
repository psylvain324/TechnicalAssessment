using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using TechnicalAssessment.Data;
using TechnicalAssessment.Models;

namespace TechnicalAssessment.Views.Transactions
{
    public class IndexModel : PageModel
    {
        private readonly DatabaseContext databaseContext;

        public IndexModel(DatabaseContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }
    }
}
