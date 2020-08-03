using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hybrid.Ai.Updater.DAL.Entities.GeoLite2
{
    [Table("update_history", Schema = "geo_lite2")]
    public class GeoLiteHistoryEntity
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