using System;
using System.IO;
using Autofac;
using Hybrid.Ai.Updater.DAL.Context;
using Hybrid.Ai.Updater.DAL.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Module = System.Reflection.Module;


namespace Hybrid.Ai.Updater.App
{
    public class Updater
    {
        private static IConfiguration _configuration;

        private static void Main(string[] args)
        {
            ConfigureServices();
            Console.WriteLine($" Hello {_configuration["version"]} !");
        }

       // private 
        private static void ConfigureServices()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true);
            _configuration = builder.Build();
            
            
            var services = new ServiceCollection();

            services
                .AddEntityFrameworkNpgsql()
                .AddDbContext<DbContext>(
                    o => o.UseNpgsql(_configuration.GetConnectionString("DataAccessPostgreSqlProvider")),
                    ServiceLifetime.Transient);

            var serviceProvider = services.BuildServiceProvider();
            
        }
    }
}