using System.Collections.Generic;
using System.Threading.Tasks;
using Hybrid.Ai.Updater.DAL.Models.Request.GeoLite.IpV4;
using Hybrid.Ai.Updater.DAL.Models.Response.GeoLite.IpV4;
using static Hybrid.Ai.Updater.Common.Models.Base.AppResponse;

namespace Hybrid.Ai.Updater.DAL.Repositories.Interfaces.GeoLite2.IpV4
{
    public interface IGeoLiteIpv4CityRepository
    {
        Task<Response<IpV4CityBaseResponse>> Get(IpV4CitySearchRequest searchRequest);
        
        Task<Response<List<IpV4CityBaseResponse>>> GetList(bool isActualize = true);

        Task<Response<List<IpV4CityBaseResponse>>> CreateRange(
            List<CityBaseRequest> entities);

        Task<Response> Update(CityBaseRequest entity);
    }
}