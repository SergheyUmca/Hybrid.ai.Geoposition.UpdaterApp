using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hybrid.Ai.Updater.DAL.Entities
{
    [Table("ipv4_information_geo_lite2", Schema = "dbo")]
    public class IpV4GeoLiteInformationEntity
    {
        public Guid Key { get; set; }
        
        [Column("autonomous_system_number")]
        public int AutonomousSystemNumber { get; set; }
        
        [Column("autonomous_system_organization"), MaxLength(250)]
        public string AutonomousSystemOrganization { get; set; }
        
        [Column("md5_sum"), MaxLength(32)]
        public string Md5Sum { get; set; }
        
        [Column("network"), MaxLength(20)]
        public string Network { get; set; }
    }
}