using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;
using ApplicationCore.Connections;
using ApplicationCore.Connections.Interfaces;
using ApplicationCore.Entities;
using DbUp;
using Infrastructure.Connections;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public class Startup
    {
        public static void Configure(string connectionString, IServiceCollection services)
        {
            services.AddScoped<IDatabaseConnection>((context) => new DatabaseConnection(new SqlConnection(connectionString)));
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString,
                sqlServerOptionsAction: sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(2), 
                        errorNumbersToAdd: null);
                }));
            services.AddIdentity<User, Role>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
        }

        public static void EnsureUpdate(string connectionString, IServiceProvider serviceProvider)
        {
            var applicationDbContext = serviceProvider.GetService<ApplicationDbContext>();
            applicationDbContext.Database.EnsureCreated();

            var databaseConnection = serviceProvider.GetRequiredService<IDatabaseConnection>();
            var upgrader = DeployChanges.To
                .SqlDatabase(connectionString)
                .JournalToSqlTable("dbo", "Migrations")
                .WithScriptsAndCodeEmbeddedInAssembly(
                    Assembly.GetExecutingAssembly(),
                    (string s) => s.Contains($".Scripts."))
                .WithTransactionPerScript()
                .LogToConsole()
                .WithExecutionTimeout(new TimeSpan(0, 0, 90))
                .Build();
            var result = upgrader.PerformUpgrade();
            if (!result.Successful)
            {
                throw new Exception(result.Error.Message);
            }
        }
    }
}
