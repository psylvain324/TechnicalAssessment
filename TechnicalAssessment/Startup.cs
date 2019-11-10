using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using TechnicalAssessment.Data;
using TechnicalAssessment.Models;

namespace TechnicalAssessment
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("V1", new OpenApiInfo { Title = "2C2P Take Home API", Version = "V1" });
            });

            // For use of external database
            //services.AddDbContext<DatabaseContext>(options =>
            //options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            //Configure in memory database and mvc routing
            services.AddDbContext<DatabaseContext>(options => options.UseInMemoryDatabase(databaseName: "TechnicalAssementDb"));
            services.AddMvc(option => option.EnableEndpointRouting = false).AddNewtonsoftJson();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "2C2P Take Home API V1");
            });

            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseRouting();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}");
            });
        }

        public static void Initialize(DatabaseContext context)
        {
            context.Database.EnsureCreated();
            if (context.Transactions.Any())
            {
                return; 
            }
            var testTransaction = new Transaction
            {
                TransactionId = "Inv00001",
                CurrencyCode = "TBH",
                Amount = 100000.00,
                Status = TransactionStatus.Approved,
                TransactionDate = DateTime.Now.ToString()
            };
            
            var testCustomer = new Customer
            {
                CustomerId = 0,
                CustomerName = "Phillip Sylvain",
                Email = "psylvain324@gmail.com",
                MobileNumber = "16032862905"
            };
            var customers = new Customer[]
            {
                testCustomer
            };

            context.Transactions.Add(testTransaction);
            context.SaveChanges();
        }
    }
}
