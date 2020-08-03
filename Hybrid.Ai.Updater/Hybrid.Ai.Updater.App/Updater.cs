using System;
using System.IO;
using Hybrid.Ai.Updater.App.Models;
using Hybrid.Ai.Updater.BLL.Handlers.Implementation.GeoLite2;
using Hybrid.Ai.Updater.BLL.Handlers.Interfaces.GeoLite2;
using Hybrid.Ai.Updater.BLL.Models.Enums;
using Hybrid.Ai.Updater.Common.Models.Constants;
using Hybrid.Ai.Updater.DAL.Context;
using Hybrid.Ai.Updater.DAL.Services.Implementation;
using Hybrid.Ai.Updater.DAL.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace Hybrid.Ai.Updater.App
{
    public static class Updater
    {
        private static IConfiguration _configuration;
        private static IServiceProvider _serviceProvider;

        private static void Main()
        {
            ConfigureServices();

            var vGeoIpSection = _configuration.GetSection("MaxMindGeoIp").Get<GeoIpConfigModel>();

            if (!vGeoIpSection.IsOk)
            {
                Console.WriteLine(ErrorMessages.AppSettingsCorrupted);
                return;
            }

            var vRemoteDbAddress =
                $"{vGeoIpSection.AddressForUpdate}?edition_id={vGeoIpSection.EditionId}&license_key={vGeoIpSection.LicenseKey}&suffix={vGeoIpSection.SuffixZip}";
            
            var vRemoteHashAddress =
                $"{vGeoIpSection.AddressForUpdate}?edition_id={vGeoIpSection.EditionId}&license_key={vGeoIpSection.LicenseKey}&suffix={vGeoIpSection.SuffixMd5}";

            
            Console.WriteLine("Please Chose type Db for Update 0)ASN 1) City");
            if (!int.TryParse(Console.ReadLine(), out var sourceType))
            {
                Console.WriteLine(ErrorMessages.InvalidRequestDataErrorMessage);
                return;
            }
            
            var handler = _serviceProvider.GetService<IGeoDbHandler>();
            var updateDb =  handler.UpdateDb((DataBaseTypes)sourceType).Result;
            if (updateDb.Data) 
                Console.WriteLine("Database successfully updated");
        }

       // private 
        private static void ConfigureServices()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true);
            _configuration = builder.Build();

             _serviceProvider = new ServiceCollection()
                .AddEntityFrameworkNpgsql()
                .AddDbContext<BaseContext>(
                    o => o.UseNpgsql(_configuration.GetConnectionString("DataAccessPostgreSqlProvider")),
                    ServiceLifetime.Transient)
                .AddSingleton<IDbService, DbService>()
                .AddSingleton<IGeoDbHandler, GeoDbHandler>()
                .BuildServiceProvider();
        }
    }
}