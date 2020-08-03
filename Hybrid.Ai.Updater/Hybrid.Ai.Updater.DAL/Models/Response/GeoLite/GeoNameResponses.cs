using System;

namespace Hybrid.Ai.Updater.DAL.Models.Response.GeoLite.IpV4
{
    public class GeoNameBaseResponse
    {
        public Guid Key { get; set; }
        
        public int GeoNameId { get; set; }
        
        public string ContinentCode { get; set; }
        
        public string ContinentName { get; set; }
        
        public string CountryIsoCode { get; set; }
        
        public string CountryName { get; set; }
        
        public string CityName { get; set; }
        
        public string Subdivision1IsoCode { get; set; }
        
        public string Subdivision1Name { get; set; }
        
        public string Subdivision2IsoCode { get; set; }
        
        public string Subdivision2Name { get; set; }
    }
}