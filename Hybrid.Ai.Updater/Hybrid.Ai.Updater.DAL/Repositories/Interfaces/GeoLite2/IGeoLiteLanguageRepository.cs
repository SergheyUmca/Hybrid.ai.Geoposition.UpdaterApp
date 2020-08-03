using System.Collections.Generic;
using System.Threading.Tasks;
using Hybrid.Ai.Updater.DAL.Models.Response.GeoLite;
using static Hybrid.Ai.Updater.Common.Models.Base.AppResponse;

namespace Hybrid.Ai.Updater.DAL.Repositories.Interfaces.GeoLite2
{
    public interface IGeoLiteLanguageRepository
    {
        Task<Response<LanguageResponse>> Get(string code);
        
        Task<Response<List<LanguageResponse>>> GetList();
        
    }
}