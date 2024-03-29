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
        public DbSet<Currency> Currencies { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Customer>(c =>
            {
                c.HasMany(t => t.Transactions)
                    .WithOne()
                    .HasForeignKey(fk => fk.CustomerId)
                    .IsRequired();
            });
        }
    }
}