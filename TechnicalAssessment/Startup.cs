using System.IO;
using System.Reflection;
using log4net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using TechnicalAssessment.Data;
using TechnicalAssessment.Models;
using TechnicalAssessment.Services;
using TechnicalAssessment.Services.Interfaces;
using Microsoft.OpenApi.Models;
using System;

namespace TechnicalAssessment
{
    public class Startup
    {
        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public static void ConfigureServices(IServiceCollection services)
        {
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            services.AddControllers();
            services.AddMvc();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "2C2P Technical Assessment", Version = "v1" });
                //c.IncludeXmlComments(xmlPath);
            });

            services.AddMvc(option => option.EnableEndpointRouting = false).AddNewtonsoftJson();
            services.AddDbContext<DatabaseContext>(options => options.UseInMemoryDatabase(databaseName: "TechnicalAssessmentDb"));
            services.AddScoped<IServiceUpload<Customer>, CustomerUploadService>();
            services.AddScoped<IServiceUpload<Transaction>, TransactionUploadService>();
            services.AddScoped<IServiceExport<Transaction>, TransactionExportService>();

            //TODO: For use of external database - Configure Azure Database
            //services.AddDbContext<DatabaseContext>(options =>
            //options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
        }

        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env, DatabaseContext databaseContext)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {                
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "2C2P Take Home API V1");
                //c.RoutePrefix = string.Empty;
            });

            app.UseMvc();
            app.UseStaticFiles(new StaticFileOptions() { FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot")) });
			app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            DataGenerator.Initialize(databaseContext);
            //TODO: Create App Identity User & Role Context
           
        }

    }
}
