using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hybrid.Ai.Updater.DAL.Models.Request.GeoLite;
using Hybrid.Ai.Updater.DAL.Models.Request.GeoLite.IpV4;
using Hybrid.Ai.Updater.DAL.Models.Response.GeoLite.IpV4;
using static Hybrid.Ai.Updater.Common.Models.Base.AppResponse;

namespace Hybrid.Ai.Updater.DAL.Repositories.Interfaces.GeoLite2
{
    public interface IGeoLiteGeoNameRepository
    {
        Task<Response<GeoNameBaseResponse>> Get(Guid historyKey, int geoNameId);
        
        Task<Response<List<GeoNameBaseResponse>>> GetList(Guid historyKey);

        Task<Response<List<GeoNameBaseResponse>>> CreateRange(
            List<GeoNameBaseRequest> directoryEntitiesList);

        Task<Response> Update(GeoNameBaseRequest entity);
    }
}