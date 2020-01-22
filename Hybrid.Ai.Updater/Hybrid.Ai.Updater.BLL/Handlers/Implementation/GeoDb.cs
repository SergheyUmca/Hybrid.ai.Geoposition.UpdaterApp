using System;
using System.Linq;
using System.Threading.Tasks;
using Hybrid.ai.Geoposition.Common.Models.BaseModels;
using Hybrid.Ai.Updater.BLL.Handlers.Interfaces;
using Hybrid.Ai.Updater.BLL.Services;
using Hybrid.Ai.Updater.Common.Models.Constants;
using Hybrid.Ai.Updater.DAL.Context;
using Hybrid.Ai.Updater.DAL.Services.Implementation;
using Hybrid.Ai.Updater.DAL.Services.Interfaces;
using static Hybrid.ai.Geoposition.Common.Models.BaseModels.Response.AppResponse;

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


        public async Task<Response<bool>> CheckForUpdates(string hashAddress)
        {
            try
            {
                var getLastHash = await FileService.GetHash(hashAddress);
                using (IDbService dbService = new DbService(_db).DbServiceInstance)
                {
                    var checkStoredHash = await dbService.GeoLite.CheckHash(getLastHash);
                    if (checkStoredHash.ResultCode != ResponseCodes.SUCCESS)
                    {
                        throw new CustomException(checkStoredHash.ResultCode,
                            checkStoredHash.Errors.FirstOrDefault()?.ResultMessage);
                    }

                    return new Response<bool>(checkStoredHash.Data);
                }
            }
            catch (CustomException ce)
            {
                Console.WriteLine(ce.Errors.FirstOrDefault()?.ResultMessage);
                throw;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        
        
        public async Task<Response<byte[]>> GetDbFile(string dbAddress/*,  string path*/ ,string fileName)
        {
            var vResult = new Response<byte[]>();
            try
            {
                var getLastDates = await FileService.DownloadDbFile(dbAddress);
                if (getLastDates.Length == 0)
                {
                    vResult.Data = getLastDates;
                    return vResult;
                }

                var unzipArchive = await ArchiveService.GetCsvFileFromZip(getLastDates, fileName);
                vResult.Data = unzipArchive;
                return vResult;
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

        public async Task<Response<bool>> UpdateDb(string dbAddress, string hashAddress, string path, string fileName)
        {
            var vResult = new Response<bool>(false);
            try
            {
                var getDbFile = await GetDbFile( dbAddress, fileName);
                if (getDbFile.Data.Length == 0)
                {
                    throw new CustomException(ResponseCodes.FILE_NOT_SAVED, ErrorMessages.FileIsCorruptErrorMessage);
                }

                var parseFile = await FileService.ParseCsvDbFile(getDbFile.Data);
                if (parseFile == null || parseFile.Count == 0)
                {
                    throw new CustomException(ResponseCodes.FAILURE, ErrorMessages.FileIsCorruptErrorMessage);
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