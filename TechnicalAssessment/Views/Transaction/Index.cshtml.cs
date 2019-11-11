using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;
using TechnicalAssessment.Data;
using TechnicalAssessment.Models;

namespace TechnicalAssessment.Views.Transactions
{
    public class IndexModel : PageModel
    {
        private readonly DatabaseContext databaseContext;
        private IList<Transaction> _Transactions;

        public IndexModel(DatabaseContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }

        public IList<Transaction> Transactions
        {
            get
            {
                _Transactions = (IList<Transaction>)databaseContext.Transactions;
                return _Transactions;
            }
        }

        public void SetTransactions(IList<Transaction> value)
        {
            _Transactions = value;
        }

        private IList<Transaction> LoadData()
        {
            return Transactions;
        }
    }
}
