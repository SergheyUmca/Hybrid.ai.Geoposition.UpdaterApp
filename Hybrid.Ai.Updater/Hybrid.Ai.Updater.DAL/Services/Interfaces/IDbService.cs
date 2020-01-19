using System;
using Hybrid.Ai.Updater.DAL.Repositories.Interfaces;
using Hybrid.Ai.Updater.DAL.Services.Implementation;

namespace Hybrid.Ai.Updater.DAL.Services.Interfaces
{
     public interface IDbService : IDisposable
    {
        DbService DbServiceInstance { get; }
        
        IGeoLite GeoLite { get; }
    }
}