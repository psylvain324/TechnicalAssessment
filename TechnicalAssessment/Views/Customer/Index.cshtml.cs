using Microsoft.AspNetCore.Mvc.RazorPages;
using TechnicalAssessment.Data;

namespace TechnicalAssessment.Views.Customer
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
