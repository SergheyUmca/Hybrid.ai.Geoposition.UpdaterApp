﻿using CsvHelper.Configuration;
 using Hybrid.Ai.Updater.BLL.ServicesModels;

 namespace Hybrid.Ai.Updater.BLL.Services
{
    public sealed class CsvParseGeoLite2Map : ClassMap<CsvParseGeoLite2>
    {
        public CsvParseGeoLite2Map()
        {
            Map(m => m.Network).Name("network");
            Map(m => m.AutonomousSystemNumber).Name("autonomous_system_number");
            Map(m => m.AutonomousSystemOrganization).Name("autonomous_system_organization");
        }
    }
}