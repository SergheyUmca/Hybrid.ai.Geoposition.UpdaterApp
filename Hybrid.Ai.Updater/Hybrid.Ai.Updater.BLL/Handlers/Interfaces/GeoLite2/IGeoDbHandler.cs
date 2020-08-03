using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hybrid.Ai.Updater.BLL.Models.Enums;
using Hybrid.Ai.Updater.BLL.Models.Service;
using Hybrid.Ai.Updater.BLL.Models.Service.GeoLite2;
using static Hybrid.Ai.Updater.Common.Models.Base.AppResponse;

namespace Hybrid.Ai.Updater.BLL.Handlers.Interfaces.GeoLite2
{
    public interface IGeoDbHandler
    {
        Task<Response<CheckUpdateResponseModel>> CheckForUpdates(DataBaseTypes sourceType);

        Task<Response<List<DbFileModel>>> GetDbFile(DataBaseTypes sourceType);
        
        Task<Response<bool>> UpdateDb(DataBaseTypes sourceType);
    }
}