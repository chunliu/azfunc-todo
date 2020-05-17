using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Chunliu.Functions.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System;

[assembly: FunctionsStartup(typeof(Chunliu.Functions.Startup))]

namespace Chunliu.Functions
{
    class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var sqlConnectionString = Environment.GetEnvironmentVariable($"ConnectionStrings:SQLConnectionString");
            if (string.IsNullOrEmpty(sqlConnectionString))
            {
                sqlConnectionString = Environment.GetEnvironmentVariable($"SQLAZURECONNSTR_SQLConnectionString");
            }

            builder.Services.AddDbContext<TodoContext>(
                options => options.UseSqlServer(sqlConnectionString));
        }
    }
}