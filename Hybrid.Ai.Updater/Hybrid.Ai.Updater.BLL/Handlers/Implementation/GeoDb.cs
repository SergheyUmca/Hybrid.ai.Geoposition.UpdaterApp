using System;
using System.Linq;
using System.Threading.Tasks;
using Hybrid.ai.Geoposition.Common.Models.BaseModels;
using Hybrid.Ai.Updater.BLL.Handlers.Interfaces;
using Hybrid.Ai.Updater.BLL.Services;
using Hybrid.Ai.Updater.Common.Models.Constants;
using Hybrid.Ai.Updater.DAL.Context;
using Hybrid.Ai.Updater.DAL.Entities;
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
        
        public async Task<Response<string>> CheckForUpdates(string hashAddress)
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

                    return new Response<string>(checkStoredHash.Data ? null : getLastHash);
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
        
        
        public async Task<Response<byte[]>> GetDbFile(string dbAddress ,string fileName)
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

        public async Task<Response<bool>> UpdateDb(string dbAddress, string hashAddress, string fileName)
        {
            var vResult = new Response<bool>(false);
            try
            {
                var getLastHash = await CheckForUpdates( hashAddress);
                if (string.IsNullOrEmpty(getLastHash.Data))
                {
                    throw new CustomException(ResponseCodes.LAST_UPDATES_ALREADY_EXISTS, ErrorMessages.DataBaseNoNeedUpdate);
                }
                
                var getDbFile = await GetDbFile( dbAddress, fileName);
                if (getDbFile.Data.Length == 0)
                {
                    throw new CustomException(ResponseCodes.FILE_NOT_SAVED, ErrorMessages.FileIsCorruptErrorMessage);
                }

                var parseFile =  FileService.ParseCsvDbFile(getDbFile.Data);
                if (parseFile == null || parseFile.Count == 0)
                {
                    throw new CustomException(ResponseCodes.FAILURE, ErrorMessages.FileIsCorruptErrorMessage);
                }
                
                using (IDbService dbService = new DbService(_db).DbServiceInstance)
                {
                    var datesForSave = parseFile.Select(s => new IpV4GeoLiteInformationEntity
                    {
                        Network = s.Network,
                        AutonomousSystemNumber = s.AutonomousSystemNumber,
                        AutonomousSystemOrganization = s.AutonomousSystemOrganization,
                        Md5Sum = getLastHash.Data
                    }).ToList();
                    var checkStoredHash = await dbService.GeoLite.CreateRange(datesForSave);
                    if (checkStoredHash.ResultCode != ResponseCodes.SUCCESS)
                    {
                        throw new CustomException(checkStoredHash.ResultCode,
                            checkStoredHash.Errors.FirstOrDefault()?.ResultMessage);
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
    }
}