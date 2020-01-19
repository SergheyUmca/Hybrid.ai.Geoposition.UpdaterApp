using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Hybrid.Ai.Updater.Common.Models.ServicesModels;

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

        public static async Task<List<GeoLiteIpModel>> ParseCsvDbFile()
        {
            return null;
        }
    }
}