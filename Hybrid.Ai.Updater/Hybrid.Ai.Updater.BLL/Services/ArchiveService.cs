using System.IO;
using System.IO.Compression;

namespace Hybrid.Ai.Updater.BLL.Services
{
    public static class ArchiveService
    {
        public static byte[] GetCsvFileFromZip(string pathToZip, string csvName)
        {
            var vMemoryStream = new MemoryStream();
            using (var zipToOpen = new FileStream(pathToZip, FileMode.Open))
            {
                using (var archive = new ZipArchive(zipToOpen, ZipArchiveMode.Update))
                {
                    var readmeEntry = archive.CreateEntry(csvName);
                    readmeEntry.Open().CopyToAsync(vMemoryStream);

                    return vMemoryStream.ToArray();
                }
            }
        }
    }
}