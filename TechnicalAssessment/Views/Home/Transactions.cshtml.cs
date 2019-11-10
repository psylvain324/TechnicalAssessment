using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TechnicalAssessment.Data;
using TechnicalAssessment.Models;

namespace TechnicalAssessment
{
    public class DetailsModel : PageModel
    {
        private readonly DatabaseContext databaseContext;

        public DetailsModel(DatabaseContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }

        public IList<Transaction> transactions { get; set; }

        public async Task OnGetAsync()
        {
            transactions = await databaseContext.Transactions
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
