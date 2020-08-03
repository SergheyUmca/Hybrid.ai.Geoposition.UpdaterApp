﻿using CsvHelper.Configuration;
 using Hybrid.Ai.Updater.BLL.Models.Service.GeoLite2;

 namespace Hybrid.Ai.Updater.BLL.Services
{
    public sealed class CsvParseAsnMap : ClassMap<CsvParserAsn>
    {
        public CsvParseAsnMap()
        {
            Map(m => m.Network).Name("network");
            Map(m => m.AutonomousSystemNumber).Name("autonomous_system_number");
            Map(m => m.AutonomousSystemOrganization).Name("autonomous_system_organization");
        }
    }
    
    public sealed class CsvParseCityMap : ClassMap<CsvParserCityBlocks>
    {
        public CsvParseCityMap()
        {
            Map(m => m.Network).Name("network");
            Map(m => m.GeoNameId).Name("geoname_id");
            Map(m => m.RegisteredCountryGeonameId).Name("registered_country_geoname_id");
            Map(m => m.RepresentedCountryGeonameId).Name("represented_country_geoname_id");
            Map(m => m.IsAnonymousProxy).Name("is_anonymous_proxy");
            Map(m => m.IsSatelliteProvider).Name("is_satellite_provider");
            Map(m => m.PostalCode).Name("postal_code");
            Map(m => m.Latitude).Name("latitude");
            Map(m => m.Longitude).Name("longitude");
            Map(m => m.AccuracyRadius).Name("accuracy_radius");
        }
    }
    
    public sealed class CsvParseCityLocationsMap : ClassMap<CsvParserCityLocations>
    {
        public CsvParseCityLocationsMap()
        {
            Map(m => m.GeoNameId).Name("geoname_id");
            Map(m => m.LocaleCode).Name("locale_code");
            Map(m => m.ContinentCode).Name("continent_code");
            Map(m => m.ContinentName).Name("continent_name");
            Map(m => m.CountryIsoCode).Name("country_iso_code");
            Map(m => m.CountryName).Name("country_name");
            Map(m => m.Subdivision1IsoCode).Name("subdivision_1_iso_code");
            Map(m => m.Subdivision1Name).Name("subdivision_1_name");
            Map(m => m.Subdivision2IsoCode).Name("subdivision_2_iso_code");
            Map(m => m.Subdivision2Name).Name("subdivision_2_name");
            Map(m => m.CityName).Name("city_name");
            Map(m => m.MetroCode).Name("metro_code");
            Map(m => m.TimeZone).Name("time_zone");
            Map(m => m.IsInEuropeanUnion).Name("is_in_european_union");
        }
    }
}