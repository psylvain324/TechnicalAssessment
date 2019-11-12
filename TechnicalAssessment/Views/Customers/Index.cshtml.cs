using Microsoft.AspNetCore.Mvc.RazorPages;
using TechnicalAssessment.Data;

namespace TechnicalAssessment.Views.Customers
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
