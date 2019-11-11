using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
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

        public IList<Transaction> Transactions { get; set; }

        public async Task OnGetAsync()
        {
            Transactions = await databaseContext.Transactions
                .Include(c => c.TransactionId)
                .AsNoTracking()
                .ToListAsync();
        }

    }
}
