using System;
using Hybrid.Ai.Updater.DAL.Context;
using Hybrid.Ai.Updater.DAL.Repositories.Implementation;
using Hybrid.Ai.Updater.DAL.Repositories.Interfaces;
using Hybrid.Ai.Updater.DAL.Services.Interfaces;

namespace Hybrid.Ai.Updater.DAL.Services.Implementation
{
    public class DbService : IDbService
    {
        private readonly BaseContext _db;
        private readonly Lazy<DbService> _lazyDbService;
        private readonly object _locker = new object();

        private IGeoLite _geoLite;

        public DbService(BaseContext db)
        {
            lock (_locker)
            {
                _db = db;
                _lazyDbService = new Lazy<DbService>(() => new DbService(_db));
            }
        }

        public DbService DbServiceInstance => _lazyDbService.Value;
     


        public IGeoLite GeoLite => _geoLite ?? (_geoLite = new GeoLite(_db));

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}