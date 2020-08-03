
namespace Hybrid.Ai.Updater.BLL.Models.Service.GeoLite2
{
    public class GeoLiteIpModel
    {
        public int AutonomousSystemNumber { get; set; }

        public string AutonomousSystemOrganization { get; set; }

        public string Md5Sum { get; set; }

        public string Ip { get; set; }
    }

    
    public class CsvParserAsn
    {
        public int AutonomousSystemNumber { get; set; }
        
        public string AutonomousSystemOrganization { get; set; }
        
        public string Network { get; set; }
    }

    public class CsvParserCityBlocks
    {
        public int GeoNameId { get; set; }
        
        public int RegisteredCountryGeonameId { get; set; }
        
        public int RepresentedCountryGeonameId { get; set; }
        
        public bool IsAnonymousProxy { get; set; }
        
        public bool IsSatelliteProvider { get; set; }
        
        public string PostalCode { get; set; }
        
        public float Latitude { get; set; }
        
        public float Longitude { get; set; }
        
        public int AccuracyRadius { get; set; }
        
        
        public string Network { get; set; }
    }

    public class CsvParserCityLocations
    {
        public int GeoNameId { get; set; }
        
        public string LocaleCode { get; set; }
        
        public string ContinentCode { get; set; }
        
        public string ContinentName { get; set; }
        
        public string CountryIsoCode { get; set; }
        
        public string CountryName { get; set; }
        
        public string CityName { get; set; }
        
        public string Subdivision1IsoCode { get; set; }
        
        public string Subdivision1Name { get; set; }
        
        public string Subdivision2IsoCode { get; set; }
        
        public string Subdivision2Name { get; set; }
        
        public string MetroCode { get; set; }
        
        public string TimeZone { get; set; }
        
        public bool IsInEuropeanUnion { get; set; }
    }
}