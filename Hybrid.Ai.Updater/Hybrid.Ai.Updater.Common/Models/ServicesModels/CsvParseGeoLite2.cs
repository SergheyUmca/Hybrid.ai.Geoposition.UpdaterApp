namespace Hybrid.Ai.Updater.Common.Models.ServicesModels
{
    public class GeoLiteIpModel
    {
        public int AutonomousSystemNumber { get; set; }

        public string AutonomousSystemOrganization { get; set; }

        public string Md5Sum { get; set; }

        public string Ip { get; set; }
    }

    public class CsvParseGeoLite2
    {
        public int AutonomousSystemNumber { get; set; }

        public string AutonomousSystemOrganization { get; set; }

        public string Md5Sum { get; set; }

        public string Network { get; set; }
    }
}