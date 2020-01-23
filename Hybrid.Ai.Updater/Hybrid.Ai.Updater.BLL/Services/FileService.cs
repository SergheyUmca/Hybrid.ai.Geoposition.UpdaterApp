using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using Hybrid.Ai.Updater.Common.Models.ServicesModels;
using LukeSkywalker.IPNetwork;

namespace Hybrid.Ai.Updater.BLL.Services
{
    public static class FileService
    {
        public static async Task<byte[]> DownloadDbFile(string address)
        {
            var webClient = new System.Net.WebClient();
            var request = await webClient.DownloadDataTaskAsync(address);
            
            return request;
        }
        
        public static async Task<string> GetHash(string address)
        {
            var webClient = new System.Net.WebClient();
            var request = await webClient.DownloadStringTaskAsync(address);
            
            return request;
        }

        public static async Task<bool> SaveFile(string path, string fileName, string address)
        {
            try
            {
                var getFile = await DownloadDbFile(address);
                
                if (getFile != null && getFile.Length > 0)
                {
                    var fullPath = path + @"\" + fileName;
                    await File.WriteAllBytesAsync(fullPath, getFile); 
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
           
        }

     
        public static List<CsvParseGeoLite2> ParseCsvDbFile( byte[] csvBody)
        {

            TextReader reader = new StreamReader(new MemoryStream(csvBody));
            var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture);
            csvConfig.RegisterClassMap<CsvParseGeoLite2Map>();
            var csvReader = new CsvReader(reader, csvConfig);
            var records =  csvReader.GetRecords<CsvParseGeoLite2>().ToList();
            
            return records;
        }
        
    }
}