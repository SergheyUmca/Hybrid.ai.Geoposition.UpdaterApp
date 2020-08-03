using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hybrid.Ai.Updater.DAL.Entities.GeoLite2.IpV4
{
    [Table("ipv4_city", Schema = "geo_lite2")]
    public class GeoLiteIpV4CityEntity
    {
        [Column("key")]
        public Guid Key { get; set; }
        
        [Column("geoname_id")]
        public int GeoNameId { get; set; }
        
        [Column("registered_country_geoname_id")]
        public int RegisteredCountryGeonameId { get; set; }
        
        [Column("represented_country_geoname_id")]
        public int RepresentedCountryGeonameId { get; set; }
        
        [Column("is_anonymous_proxy")]
        public bool IsAnonymousProxy { get; set; }
        
        [Column("is_satellite_provider")]
        public bool IsSatelliteProvider { get; set; }
        
        [Column("postal_code"), MaxLength(12)]
        public string PostalCode { get; set; }
        
        [Column("latitude")]
        public float Latitude { get; set; }
        
        [Column("longitude")]
        public float Longitude { get; set; }
        
        [Column("accuracy_radius")]
        public int AccuracyRadius { get; set; }

        [Column("md5_sum"), MaxLength(32)]
        public string Md5Sum { get; set; }
        
        [Column("history_key")]
        public Guid HistoryKey { get; set; }
        
        [Column("network"), MaxLength(50)]
        public string Network { get; set; }
        
        [Column("cidr")]
        public int Cidr { get; set; }
        
        [Column("min_first_segment")]
        public int MinFirstSegment { get; set; }
        
        [Column("min_second_segment")]
        public int MinSecondSegment { get; set; }
        
        [Column("min_third_segment")]
        public int MinThirdSegment { get; set; }
        
        [Column("min_last_segment")]
        public int MinLastSegment { get; set; }
        
        [Column("max_first_segment")]
        public int MaxFirstSegment { get; set; }
        
        [Column("max_second_segment")]
        public int MaxSecondSegment { get; set; }
        
        [Column("max_third_segment")]
        public int MaxThirdSegment { get; set; }
        
        [Column("max_last_segment")]
        public int MaxLastSegment { get; set; }
    }
}