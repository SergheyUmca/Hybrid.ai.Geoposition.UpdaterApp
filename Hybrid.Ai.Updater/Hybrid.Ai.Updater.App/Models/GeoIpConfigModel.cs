using CsvHelper.Configuration.Attributes;

namespace Hybrid.Ai.Updater.App.Models
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public  class GeoIpConfigModel
    {
        public string AddressForUpdate { get; set; }
        
        public string LicenseKey { get; set; }
        
        public string EditionId { get; set; }
        
        public string SuffixZip { get; set; }
        
        public string SuffixMd5 { get; set; }
        
        public string CsvName { get; set; }
        
        public bool IsOk
        {
            get
            {
                var isOk = !string.IsNullOrEmpty(AddressForUpdate) && !string.IsNullOrEmpty(LicenseKey) &&
                           !string.IsNullOrEmpty(EditionId) && !string.IsNullOrEmpty(SuffixZip) &&
                           !string.IsNullOrEmpty(SuffixMd5) && !string.IsNullOrEmpty(CsvName);
                return isOk;
            }
        }
        
    }
}