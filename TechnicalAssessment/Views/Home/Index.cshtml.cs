using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TechnicalAssessment.Controllers;
using TechnicalAssessment.Data;
using TechnicalAssessment.Models;

namespace TechnicalAssessment.Views.Home
{
    public class IndexModel : PageModel
    {
        private readonly DatabaseContext databaseContext;
        private readonly ILogger<TransactionController> logger;
        private TransactionService uploadService = new TransactionService();

        public IndexModel(DatabaseContext databaseContext, TransactionService uploadService, ILogger<TransactionController> logger)
        {
            this.databaseContext = databaseContext;
            this.uploadService = uploadService;
            this.logger = logger;
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
            try
            {
                uploadService.UploadTransaction(filePath);
            }
            catch(Exception e)
            {
                logger.LogInformation(e.Message);
            }

            return RedirectToAction("Index");
        }
    }
}
