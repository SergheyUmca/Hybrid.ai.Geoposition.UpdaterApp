using System;
using System.Linq;
using System.Threading.Tasks;
using Hybrid.ai.Geoposition.Common.Models.BaseModels;
using Hybrid.ai.Geoposition.Common.Models.Constants;
using Hybrid.Ai.Updater.BLL.Handlers.Interfaces;
using Hybrid.Ai.Updater.BLL.Services;
using Hybrid.Ai.Updater.DAL.Context;
using Hybrid.Ai.Updater.DAL.Services.Implementation;
using Hybrid.Ai.Updater.DAL.Services.Interfaces;
using static Hybrid.ai.Geoposition.Common.Models.BaseModels.Response.AppResponse;
using Response = Hybrid.ai.Geoposition.Common.Models.BaseModels.Response;

namespace Hybrid.Ai.Updater.BLL.Handlers.Implementation
{
    public class GeoDb : IGeoDb
    {
        
        private readonly BaseContext _db;

        public GeoDb(BaseContext context)
        {
            _db = context;
        }
        
        
        public async Task<Response<string>> GetDbInfo()
        {
            try
            {
                var result = await FileService.GetHash("");
                return new Response<string>(result);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task<Response<bool>> GetDbFile()
        {
            var vResult = new Response<bool>(false);
            try
            {
                var getLastHash = await FileService.GetHash("");
                using (IDbService dbService = new DbService(_db).DbServiceInstance)
                {
                    var checkStoredHash = await dbService.GeoLite.CheckHash(getLastHash);
                    if (checkStoredHash.ResultCode != ResponseCodes.SUCCESS)
                    {
                        throw new CustomException(checkStoredHash.ResultCode,
                            checkStoredHash.Errors.FirstOrDefault()?.ResultMessage);
                    }

                    if (!checkStoredHash.Data)
                    {
                        var getLastDates = await FileService.SaveFile("", "", "");
                        if (getLastDates)
                        {
                            vResult.Data = true;
                        }
                    }
                }
            }
            catch (CustomException ce)
            {
                Console.WriteLine(ce.Errors.FirstOrDefault()?.ResultMessage);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return vResult;
        }

        public async Task<Response<bool>> UpdateDb()
        {
            var vResult = new Response<bool>(false);
            try
            {
                var getDbFile = await GetDbFile();
                if (!getDbFile.Data)
                {
                    throw new CustomException(ResponseCodes.FILE_NOT_SAVED, ErrorMessages.FileIsCorruptErrorMessage);
                }
                
                    
            }
            catch (CustomException ce)
            {
                Console.WriteLine(ce.Errors.FirstOrDefault()?.ResultMessage);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return vResult;
        }
    }
}