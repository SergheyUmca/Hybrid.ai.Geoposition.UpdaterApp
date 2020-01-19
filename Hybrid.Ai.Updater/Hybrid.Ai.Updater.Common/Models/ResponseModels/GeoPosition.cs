namespace Hybrid.ai.Geoposition.Common.Models.ResponseModel
{
    public abstract class GeoPosition
    {
        public string IpAddress { get; set; }
        public string CountryCode { get; set; }
        public string CountryName { get; set; }
        public string CityName { get; set; }
    }
}