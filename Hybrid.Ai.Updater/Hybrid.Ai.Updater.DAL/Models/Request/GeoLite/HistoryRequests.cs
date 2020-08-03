using System;

namespace Hybrid.Ai.Updater.DAL.Models.Request.GeoLite
{
    public class HistoryBaseRequest
    {
        public Guid Key { get; set; }
        
        public string Md5Sum { get; set; }
        
        public DateTime LastCheckDate { get; set; }
        
        public DateTime UpdateDate { get; set; }
        
        public bool Actualize { get; set; }
    }
}