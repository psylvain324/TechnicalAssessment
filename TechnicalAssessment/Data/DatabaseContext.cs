using Microsoft.EntityFrameworkCore;
using TechnicalAssessment.Models;
using TechnicalAssessment.Models.ViewModels;

namespace TechnicalAssessment.Data
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options)
          : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<CurrencyViewModel> Currencies { get; set; }
        public DbSet<UploadFile> Files { get; set; }
    }
}
