using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hybrid.Ai.Updater.DAL.Entities
{
    [Table("ipv4_update_history_geo_lite2", Schema = "base")]
    public class IpV4GeoLiteHistoryEntity
    {
        [Column("key")]
        public Guid Key { get; set; }

        [Column("md5_sum"), MaxLength(32)]
        public string Md5Sum { get; set; }
        
        [Column("last_check_date")]
        public DateTime LastCheckDate { get; set; }
        
        [Column("update_date")]
        public DateTime UpdateDate { get; set; }
        
        [Column("actualize")]
        public bool Actualize { get; set; }
    }
}