using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using TechnicalAssessment.Data;
using TechnicalAssessment.Models;

namespace TechnicalAssessment.Views.Home
{
    public class IndexModel : PageModel
    {
        private readonly DatabaseContext databaseContext;
        private UploadService uploadService;

        public IndexModel(DatabaseContext databaseContext, UploadService uploadService)
        {
            this.databaseContext = databaseContext;
            this.uploadService = uploadService;
        }

        public IList<Transaction> Transactions { get; set; }

        public async Task OnGetAsync()
        {
            Transactions = await databaseContext.Transactions
                .Include(c => c.TransactionId)
                .AsNoTracking()
                .ToListAsync();
        }

        public IActionResult OnPostUpload(string filePath)
        {
            uploadService.uploadTransaction(filePath);
            return RedirectToAction("Index");
        }
    }
}
