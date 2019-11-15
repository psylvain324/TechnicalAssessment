using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using TechnicalAssessment.Data;

namespace TechnicalAssessment
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("V1", new OpenApiInfo { Title = "2C2P Take Home API", Version = "V1" });
                /*
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                try
                {
                    c.IncludeXmlComments(xmlPath);
                }
                catch (Exception) { }
                */
            });

            // For use of external database
            //services.AddDbContext<DatabaseContext>(options =>
            //options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddMvc(option => option.EnableEndpointRouting = false).AddNewtonsoftJson();
            services.AddDbContext<DatabaseContext>(options => options.UseInMemoryDatabase(databaseName: "TechnicalAssessmentDb"));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, DatabaseContext databaseContext)
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
            });

            app.UseStaticFiles();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                /*routes.MapRoute(
                    name: "Transaction",
                    template: "{controller=Transaction}/{action=Index}/{id?}");

                routes.MapRoute(
                    name: "Customer",
                    template: "{controller=Customer}/{action=Index}/{id?}");*/

            });
            DataGenerator.Initialize(databaseContext);
        }

    }
}
