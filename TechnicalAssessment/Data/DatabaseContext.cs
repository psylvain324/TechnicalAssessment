﻿using Microsoft.EntityFrameworkCore;
using TechnicalAssessment.Models;

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
    }
}