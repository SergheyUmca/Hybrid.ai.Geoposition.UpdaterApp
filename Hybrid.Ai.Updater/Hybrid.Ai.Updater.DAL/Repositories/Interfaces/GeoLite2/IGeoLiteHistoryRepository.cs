using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hybrid.Ai.Updater.DAL.Models.Request.GeoLite;
using Hybrid.Ai.Updater.DAL.Models.Response.GeoLite;
using static Hybrid.Ai.Updater.Common.Models.Base.AppResponse;

namespace Hybrid.Ai.Updater.DAL.Repositories.Interfaces.GeoLite2
{
    public interface IGeoLiteHistoryRepository
    {
        Task<Response<HistoryBaseResponse>> Get(Guid key);
        
        Task<Response<List<HistoryBaseResponse>>> GetList(bool isLast = false);

        Task<Response<HistoryBaseResponse>> CheckHash(string md5Hash);

        Task<Response> Update(HistoryBaseRequest entity);
    }
}