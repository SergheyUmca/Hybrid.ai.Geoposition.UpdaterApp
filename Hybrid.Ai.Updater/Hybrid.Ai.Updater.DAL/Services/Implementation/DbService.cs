using System;
using Hybrid.Ai.Updater.DAL.Context;
using Hybrid.Ai.Updater.DAL.Repositories.Implementation.GeoLite2;
using Hybrid.Ai.Updater.DAL.Repositories.Implementation.GeoLite2.IpV4;
using Hybrid.Ai.Updater.DAL.Repositories.Interfaces.GeoLite2;
using Hybrid.Ai.Updater.DAL.Repositories.Interfaces.GeoLite2.IpV4;
using Hybrid.Ai.Updater.DAL.Services.Interfaces;

namespace Hybrid.Ai.Updater.DAL.Services.Implementation
{
    public class DbService : IDbService
    {
        private readonly BaseContext _db;
        private readonly Lazy<DbService> _lazyDbService;
        private readonly object _locker = new object();

        private IGeoLiteIpV4AsnRepository _geoLiteIpV4AsnRepository;
        private IGeoLiteIpv4CityRepository _geoLiteIpv4CityRepository;
        private IGeoLiteGeoNameRepository _geoLiteGeoNameRepository;
        private IGeoLiteHistoryRepository _geoLiteHistoryRepository;
        private IGeoLiteLanguageRepository _geoLiteLanguageRepository;

        public DbService(BaseContext db)
        {
            lock (_locker)
            {
                _db = db;
                _lazyDbService = new Lazy<DbService>(() => new DbService(_db));
            }
        }

        public DbService DbServiceInstance => _lazyDbService.Value;

        
        public IGeoLiteIpV4AsnRepository GeoLiteIpV4Asn =>
            _geoLiteIpV4AsnRepository ?? (_geoLiteIpV4AsnRepository = new GeoLiteIpV4AsnRepository(_db));

        public IGeoLiteIpv4CityRepository GeoLiteIpv4City =>
            _geoLiteIpv4CityRepository ?? (_geoLiteIpv4CityRepository = new GeoLiteIpv4CityRepository(_db));

        public IGeoLiteHistoryRepository GeoLiteHistory =>
            _geoLiteHistoryRepository ?? (_geoLiteHistoryRepository = new GeoLiteHistoryRepository(_db));

        public IGeoLiteLanguageRepository GeoLiteLanguage =>
            _geoLiteLanguageRepository ?? (_geoLiteLanguageRepository = new GeoLiteLanguageRepository(_db));
        
        public IGeoLiteGeoNameRepository GeoLiteGeoName =>
            _geoLiteGeoNameRepository ?? (_geoLiteGeoNameRepository = new GeoLiteGeoNameRepository(_db));
        

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}