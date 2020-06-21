namespace Hybrid.Ai.Updater.App.Models.ResponseModels
{
    public abstract class GeoPosition
    {
        public string IpAddress { get; set; }
        public string CountryCode { get; set; }
        public string CountryName { get; set; }
        public string CityName { get; set; }
    }
}