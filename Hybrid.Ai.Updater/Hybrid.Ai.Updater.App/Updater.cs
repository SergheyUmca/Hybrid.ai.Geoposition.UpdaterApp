using System;
using System.IO;
using System.Linq;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Hybrid.Ai.Updater.BLL.Handlers.Implementation;
using Hybrid.Ai.Updater.BLL.Handlers.Interfaces;
using Hybrid.Ai.Updater.Common.Models.Constants;
using Hybrid.Ai.Updater.DAL.Context;
using Hybrid.Ai.Updater.DAL.Services.Implementation;
using Hybrid.Ai.Updater.DAL.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace Hybrid.Ai.Updater.App
{
    public class Updater
    {
        private static IConfiguration _configuration;
        private static IServiceProvider _serviceProvider;

        private static void Main(string[] args)
        {
            ConfigureServices();

            var vGeoIpSection = _configuration.GetSection("MaxMindGeoIp").GetChildren().ToList();

            var vRemoteAddressPrefix = vGeoIpSection.FirstOrDefault(f => f.Key.Equals("addressForUpdate"))?.Value;
            var vLicenseKey = vGeoIpSection.FirstOrDefault(f => f.Key.Equals("LicenseKey"))?.Value;
            var vEditionId =  vGeoIpSection.FirstOrDefault(f => f.Key.Equals("edition_id"))?.Value;
            var vSuffixZip =  vGeoIpSection.FirstOrDefault(f => f.Key.Equals("suffixZip"))?.Value;
            var vSuffixMd5 =  vGeoIpSection.FirstOrDefault(f => f.Key.Equals("suffixMd5"))?.Value;
            var vFileName = vGeoIpSection.FirstOrDefault(f => f.Key.Equals("CsvName"))?.Value;

            if (string.IsNullOrEmpty(vRemoteAddressPrefix) || string.IsNullOrEmpty(vLicenseKey)
                                                           || string.IsNullOrEmpty(vEditionId) ||
                                                           string.IsNullOrEmpty(vSuffixZip)
                                                           || string.IsNullOrEmpty(vSuffixMd5) ||
                                                           string.IsNullOrEmpty(vFileName))
            {
                Console.WriteLine(ErrorMessages.AppSettingsCorrupted);
                return;
            }

            var vRemoteDbAddress =
                $"{vRemoteAddressPrefix}?edition_id={vEditionId}&license_key={vLicenseKey}&suffix={vSuffixZip}";
            
            var vRemoteHashAddress =
                $"{vRemoteAddressPrefix}?edition_id={vEditionId}&license_key={vLicenseKey}&suffix={vSuffixMd5}";

            var handler = _serviceProvider.GetService<IGeoDb>();
            var gedFile =  handler.UpdateDb(vRemoteDbAddress, vRemoteHashAddress, Directory.GetCurrentDirectory(), vFileName).Result;
            Console.WriteLine($" Hello {_configuration["version"]} !");
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
                .AddSingleton<IGeoDb, GeoDb>()
                .BuildServiceProvider();
        }
    }
}