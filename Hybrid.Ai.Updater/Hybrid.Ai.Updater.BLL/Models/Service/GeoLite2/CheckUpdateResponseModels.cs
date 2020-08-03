using System;

namespace Hybrid.Ai.Updater.BLL.Models.Service
{
    public class CheckUpdateResponseModel
    {
        public Guid? HistoryRecordKey { get; set; }
        public string LastHashValue { get; set; }
    }
}