using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hybrid.Ai.Updater.DAL.Models.Request.GeoLite.IpV4;
using Hybrid.Ai.Updater.DAL.Models.Response.GeoLite.IpV4;
using static Hybrid.Ai.Updater.Common.Models.Base.AppResponse;

namespace Hybrid.Ai.Updater.DAL.Repositories.Interfaces.GeoLite2.IpV4
{
    public interface IGeoLiteIpV4AsnRepository
    {
        Task<Response<Ipv4AsnBaseResponse>> Get(IpV4AsnSearchRequest searchRequest);
        
        Task<Response<List<Ipv4AsnBaseResponse>>> GetList(Guid? historyKey = null);

        Task<Response<List<Ipv4AsnBaseResponse>>> CreateRange(
            List<Ipv4AsnBaseRequest> directoryEntitiesList);

        Task<Response> Update(Ipv4AsnBaseRequest entity);
    }
}