using Microsoft.AspNetCore.Mvc.RazorPages;
using TechnicalAssessment.Data;

namespace TechnicalAssessment.Views.Home
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
