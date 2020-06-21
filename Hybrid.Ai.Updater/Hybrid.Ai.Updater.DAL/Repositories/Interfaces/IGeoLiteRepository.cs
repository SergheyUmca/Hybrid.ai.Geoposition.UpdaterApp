using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hybrid.Ai.Updater.DAL.Entities;
using static Hybrid.ai.Geoposition.Common.Models.BaseModels.Response.AppResponse;

namespace Hybrid.Ai.Updater.DAL.Repositories.Interfaces
{
    public interface IGeoLiteRepository
    {
        Task<Response<IpV4GeoLiteInformationEntity>> Get(string md5Hash, string network);
        
        Task<Response<List<IpV4GeoLiteInformationEntity>>> GetList(string md5Hash);

        Task<Response<Guid?>> CheckHash(string md5Hash);

        Task<Response<List<IpV4GeoLiteInformationEntity>>> CreateRange(
            List<IpV4GeoLiteInformationEntity> directoryEntitiesList);

        Task<Response> Update(IpV4GeoLiteInformationEntity entity);
    }
}