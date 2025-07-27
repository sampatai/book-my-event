using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Wolverine;
using Wolverine.FluentValidation;
using Wolverine.Postgresql;
using Wolverine.EntityFrameworkCore;
namespace SharedKernel;

public static class WolverineExtensions
{
    public static IHostBuilder AddWolverineApplication(
        this IHostBuilder hostBuilder,
        IConfiguration configuration,
        string connectionStringName)
    {
        return hostBuilder.UseWolverine(opts =>
        {
            opts.UseFluentValidation();

            // Use the provided generic DbContext type
            opts.UseEntityFrameworkCoreTransactions();

            // Get the connection string dynamically
            string connectionString = configuration.GetConnectionString(connectionStringName)
                                 ?? throw new InvalidOperationException($"Connection string '{connectionStringName}' not found.");

            // Configure Wolverine persistence with PostgreSQL
            opts.PersistMessagesWithPostgresql(connectionString, "wolverine");

            // Durability for development
            opts.Durability.Mode = DurabilityMode.Solo;

            // Register open generic middleware
            opts.Policies.AddMiddleware(typeof(LoggingMiddleware<>));
        });
    }
}

