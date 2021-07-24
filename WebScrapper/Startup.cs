using System;
using System.Text;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using WebScrapper.JWT;
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
            String key = "Very complex key";

            services.AddAuthentication(x =>
            {
                x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
        //    services.AddDbContext<DBContext>(options =>
        //        options.UseSqlServer(
        //            "Server=localhost;Database=ScrapperDk;Trusted_Connection=True;MultipleActiveResultSets=true"));
       //     services.AddEntityFrameworkSqlServer().AddDbContext<DBContext>();
            services.AddSingleton<IJwtAuthenticationManager>(new JwtAuthenticationManager(key));
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
                options.AddPolicy("CorsPolicy",
                    policy => { policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader(); });
            });


            services.AddHangfireServer();
            services.AddScoped<IScrapperService, ScrapperService>();
            services.AddScoped<ILogInService, LogInService>();
            services.AddScoped<Starter>();
          //  String connection = Configuration.GetConnectionString("MyConnectionString");
            services.AddDbContext<DBContext>(options => options.UseSqlServer(Configuration["Data:ScrapperDatabase:ConnectionString"]));
            var sp = services.BuildServiceProvider();
            //
            // //3.Resolve the services from the service provider
              var myDbContext = sp.GetService<DBContext>();
              if (myDbContext != null) 
                myDbContext.Database.Migrate();
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,
            IBackgroundJobClient backgroundJobClient, IRecurringJobManager recurringJobManager,
            IServiceProvider serviceProvider, DBContext dbContext)
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

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCors("CorsPolicy");

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
           // Starter starter = new Starter(dbContext);
           // starter.Start();
             backgroundJobClient.Enqueue(() => Console.WriteLine("Handfire job"));
                    recurringJobManager.AddOrUpdate("Run every day", () => serviceProvider.GetService<Starter>().Start(), Cron.Daily);
        }
    }
}