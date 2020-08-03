using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hybrid.Ai.Updater.DAL.Entities.GeoLite2
{
    [Table("language", Schema = "geo_lite2")]
    public class LanguageEntity
    {
        [Column("key")]
        public Guid Key { get; set; }
        
        [Column("name"), MaxLength(1024)]
        public string Name { get; set; }
        
        [Column("code"), MaxLength(4)]
        public string Code { get; set; }
    }
}