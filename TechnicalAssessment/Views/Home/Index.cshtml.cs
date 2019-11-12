using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using TechnicalAssessment.Controllers;
using TechnicalAssessment.Data;

namespace TechnicalAssessment.Views.Home
{
    public class IndexModel : PageModel
    {
        private readonly DatabaseContext databaseContext;
        private readonly ILogger<HomeController> homeLogger;

        public IndexModel(DatabaseContext databaseContext, ILogger<HomeController> homeLogger)
        {
            this.databaseContext = databaseContext;
            this.homeLogger = homeLogger;
        }

    }
}
