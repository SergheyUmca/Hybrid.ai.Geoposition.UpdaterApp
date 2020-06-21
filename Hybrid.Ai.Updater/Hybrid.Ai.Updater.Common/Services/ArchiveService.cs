using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace Hybrid.Ai.Updater.Common.Services
{
    public static class ArchiveService
    {
        public static async Task<byte[]> GetCsvFileFromZip(string pathToZip, string csvName)
        {
            var vMemoryStream = new MemoryStream();
            using (var zipToOpen = new FileStream(pathToZip, FileMode.Open))
            {
                using (var archive = new ZipArchive(zipToOpen, ZipArchiveMode.Update))
                {
                    var readmeEntry = archive.CreateEntry(csvName);
                    await readmeEntry.Open().CopyToAsync(vMemoryStream);

                    return vMemoryStream.ToArray();
                }
            }
        }
        
        public static async Task<byte[]> GetCsvFileFromZip(byte[] zipBody, string csvName)
        {
            var vMemoryStream = new MemoryStream();
            using (var zipToOpen = new MemoryStream( zipBody))
            {
                using (var archive =  new ZipArchive(zipToOpen, ZipArchiveMode.Read))
                {
                    foreach (var entry in archive.Entries)
                    {
                        if (!entry.Name.EndsWith(csvName)) 
                            continue;
                        
                        await entry.Open().CopyToAsync(vMemoryStream);
                        
                        return vMemoryStream.ToArray();
                    }

                    return null;
                }
            }
        }
    }
}