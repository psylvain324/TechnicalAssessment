using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using TechnicalAssessment.Data;
using TechnicalAssessment.Models;

namespace TechnicalAssessment.Views.Pages
{
    public class IndexModel : PageModel
    {
        private readonly DatabaseContext databaseContext;

        public IndexModel(DatabaseContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }

        public IList<Transaction> Course { get; set; }

        public async Task OnGetAsync()
        {
            Course = await databaseContext.Transactions
                .Include(c => c.TransactionId)
                .AsNoTracking()
                .ToListAsync();
        }

    }
}
