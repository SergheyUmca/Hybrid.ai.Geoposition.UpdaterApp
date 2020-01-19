using System.Threading.Tasks;
using static Hybrid.ai.Geoposition.Common.Models.BaseModels.Response.AppResponse;
using Response = Hybrid.ai.Geoposition.Common.Models.BaseModels.Response;

namespace Hybrid.Ai.Updater.BLL.Handlers.Interfaces
{
    public interface IGeoDb
    {
        Task<Response<string>> GetDbInfo();

        Task<Response<bool>> GetDbFile();
        
        Task<Response<bool>> UpdateDb();
    }
}