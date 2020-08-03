using System;

namespace Hybrid.Ai.Updater.DAL.Models.Response.GeoLite.IpV4
{
    public class IpV4CityBaseResponse
    {
        public Guid Key { get; set; }
        
        public int GeoNameId { get; set; }
        
        public int RegisteredCountryGeonameId { get; set; }
        
        public int RepresentedCountryGeonameId { get; set; }
        
        public bool IsAnonymousProxy { get; set; }
        
        public bool IsSatelliteProvider { get; set; }
        
        public string PostalCode { get; set; }
        
        public float Latitude { get; set; }
        
        public float Longitude { get; set; }
        
        public int AccuracyRadius { get; set; }
        
        public string Md5Sum { get; set; }
        
        public Guid HistoryKey { get; set; }
        
        public string Network { get; set; }
        
        public int Cidr { get; set; }
        
        public int MinFirstSegment { get; set; }
        
        public int MinSecondSegment { get; set; }
        
        public int MinThirdSegment { get; set; }
        
        public int MinLastSegment { get; set; }
        
        public int MaxFirstSegment { get; set; }
        
        public int MaxSecondSegment { get; set; }
        
        public int MaxThirdSegment { get; set; }
        
        public int MaxLastSegment { get; set; }
        
        public bool IsActualize { get; set; }
    }
}