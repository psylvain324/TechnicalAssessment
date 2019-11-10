using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechnicalAssessment.Data;

namespace TechnicalAssessment.Views.Pages
{
    public class IndexSelectModel : PageModel
    {
        private readonly DatabaseContext databaseContext;

        public IndexSelectModel(DatabaseContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }

        public IList<TransactionViewModel> transactionViewModels { get; set; }

        public async Task OnGetAsync()
        { 
            transactionViewModels = await databaseContext.Transactions
                    .Select(p => new TransactionViewModel
                    {
                        TransactionId = p.TransactionId,
                        Amount = p.Amount,
                        CurrencyCode = p.CurrencyCode,
                        TransactionDate = p.TransactionDate,
                        Status = p.Status
                    }).ToListAsync();
        }
    }
}
