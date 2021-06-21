using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;

using LinkShortenerAPI.Models;
using LinkShortenerAPI.Repositories;

namespace LinkShortenerAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddJsonFile("appsettings.secrets.json")
            .Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var databaseSettings = new DatabaseSettings() {
                ConnectionString = Configuration["DatabaseSettings:ConnectionString"],
                UsersDatabaseName = Configuration["DatabaseSettings:UsersDatabaseName"],
                LinksDatabaseName = Configuration["DatabaseSettings:LinksDatabaseName"],
            };

            services.AddSingleton<DatabaseSettings>(sp =>
                databaseSettings);

            var urlSettings = new UrlSettings()
            {
                BaseUrl = Configuration["BaseUrl"],
            };

            services.AddSingleton<UrlSettings>(sp =>
                urlSettings);

            services.AddSingleton<IMongoClient, MongoClient>(sp =>
                new MongoClient(databaseSettings.ConnectionString));

            // Cookie auth setup
            services.AddAuthentication("CookieAuth")
                .AddCookie("CookieAuth", options =>
                {
                    options.LoginPath = new Microsoft.AspNetCore.Http.PathString("/api/v1/users/login");
                    options.ExpireTimeSpan = TimeSpan.FromHours(2);
                });

            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<ILinkReferenceRepository, LinkReferenceRepository>();

            services.AddControllers();

            services.AddApiVersioning();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
