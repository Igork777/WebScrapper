using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using WebScrapper.Scraping;
using WebScrapper.Scraping.ScrappingFluggerDk.DB;
using WebScrapper.Services;


namespace WebScrapper
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
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "WebScrapper", Version = "v1"});
            });
            services.AddHangfire(config => config.SetDataCompatibilityLevel
                    (CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseDefaultTypeSerializer()
                .UseMemoryStorage());
            
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", policy =>
                {
                    policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                });
            });
            
            

            services.AddHangfireServer();
            services.AddScoped<IScrapperService, ScrapperService>();
            services.AddEntityFrameworkSqlite().AddDbContext<DBContext>();

            Starter starter = new Starter();
            starter.Start();
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IBackgroundJobClient backgroundJobClient, IRecurringJobManager recurringJobManager, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebScrapper v1"));
            }

            app.UseHttpsRedirection();
            app.UseHangfireDashboard();
            app.UseRouting();

            app.UseAuthorization();
            app.UseCors("CorsPolicy");

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

            
            backgroundJobClient.Enqueue(() => Console.WriteLine("Handfire job"));
            // recurringJobManager.AddOrUpdate("Run every minute",
            //     () =>  
            //         serviceProvider.GetService<ScrapperFluggerDk>().StartScrapping()
            //     , "* * * * *");
        }
    }
}