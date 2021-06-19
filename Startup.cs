using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
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

            services.AddSingleton<IMongoClient, MongoClient>(sp =>
                new MongoClient(databaseSettings.ConnectionString));

            services.AddTransient<IUserRepository, UserRepository>();

            services.AddControllers();
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

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
