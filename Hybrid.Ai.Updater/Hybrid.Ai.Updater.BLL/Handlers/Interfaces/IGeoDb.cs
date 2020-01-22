using System.Threading.Tasks;
using static Hybrid.ai.Geoposition.Common.Models.BaseModels.Response.AppResponse;

namespace Hybrid.Ai.Updater.BLL.Handlers.Interfaces
{
    public interface IGeoDb
    {
        Task<Response<string>> GetDbInfo();

        Task<Response<bool>> CheckForUpdates(string hashAddress);

        Task<Response<byte[]>> GetDbFile(string dbAddress, string fileName);
        
        Task<Response<bool>> UpdateDb(string dbAddress, string hashAddress, string path, string fileName);
    }
}