using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hybrid.Ai.Updater.DAL.Entities.GeoLite2
{
    [Table("geo_name", Schema = "geo_lite2")]
    public class GeoNameEntity
    {
        [Column("key")]
        public Guid Key { get; set; }
        
        [Column("geoname_id")]
        public int GeoNameId { get; set; }
        
        [Column("continent_code"), MaxLength(4)]
        public string ContinentCode { get; set; }
        
        [Column("continent_name")]
        public string ContinentName { get; set; }
        
        [Column("country_iso_code"), MaxLength(4)]
        public string CountryIsoCode { get; set; }
        
        [Column("country_name")]
        public string CountryName { get; set; }
        
        [Column("city_name")]
        public string CityName { get; set; }
        
        [Column("subdivision_1_iso_code"), MaxLength(4)]
        public string Subdivision1IsoCode { get; set; }
        
        [Column("subdivision_1_name")]
        public string Subdivision1Name { get; set; }
        
        [Column("subdivision_2_iso_code"), MaxLength(4)]
        public string Subdivision2IsoCode { get; set; }
        
        [Column("subdivision_2_name")]
        public string Subdivision2Name { get; set; }
        
        [Column("history_key")]
        public Guid HistoryKey { get; set; }
    }
}