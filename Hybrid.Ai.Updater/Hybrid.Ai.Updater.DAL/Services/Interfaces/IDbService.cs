using System;
using Hybrid.Ai.Updater.DAL.Repositories.Interfaces.GeoLite2;
using Hybrid.Ai.Updater.DAL.Repositories.Interfaces.GeoLite2.IpV4;
using Hybrid.Ai.Updater.DAL.Services.Implementation;

namespace Hybrid.Ai.Updater.DAL.Services.Interfaces
{
     public interface IDbService : IDisposable
    {
        DbService DbServiceInstance { get; }
        
        IGeoLiteIpV4AsnRepository GeoLiteIpV4Asn { get; }
        
        IGeoLiteIpv4CityRepository GeoLiteIpv4City { get; }
        
        IGeoLiteGeoNameRepository GeoLiteGeoName { get; }
        
        IGeoLiteHistoryRepository GeoLiteHistory { get; }
        
        IGeoLiteLanguageRepository GeoLiteLanguage { get; }
    }
}