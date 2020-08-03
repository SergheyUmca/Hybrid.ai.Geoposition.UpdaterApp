using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hybrid.Ai.Updater.DAL.Entities.GeoLite2.IpV4
{
    [Table("ipv4_ASN", Schema = "geo_lite2")]
    public class GeoLiteIpV4AsnEntity
    {
        [Column("key")]
        public Guid Key { get; set; }
        
        [Column("autonomous_system_number")]
        public int AutonomousSystemNumber { get; set; }
        
        [Column("autonomous_system_organization"), MaxLength(250)]
        public string AutonomousSystemOrganization { get; set; }
        
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