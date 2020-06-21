using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hybrid.ai.Geoposition.Common.Models.BaseModels;
using Hybrid.Ai.Updater.BLL.Handlers.Interfaces;
using Hybrid.Ai.Updater.BLL.Services;
using Hybrid.Ai.Updater.Common.Models.Constants;
using Hybrid.Ai.Updater.Common.Services;
using Hybrid.Ai.Updater.DAL.Context;
using Hybrid.Ai.Updater.DAL.Entities;
using Hybrid.Ai.Updater.DAL.Services.Implementation;
using Hybrid.Ai.Updater.DAL.Services.Interfaces;
using static Hybrid.ai.Geoposition.Common.Models.BaseModels.Response.AppResponse;

namespace Hybrid.Ai.Updater.BLL.Handlers.Implementation
{
    public class GeoDbHandler : IGeoDbHandler
    {
        
        private readonly BaseContext _db;

        public GeoDbHandler(BaseContext context)
        {
            _db = context;
        }
        
        public async Task<Response<KeyValuePair<Guid, string>>> CheckForUpdates(string hashAddress)
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

                    return new Response<KeyValuePair<Guid, string>>(checkStoredHash.Data != null ? new 
                    KeyValuePair<Guid, string>((Guid)checkStoredHash.Data, getLastHash) : new KeyValuePair<Guid, string>());
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
                if (Equals(getLastHash.Data, new KeyValuePair<Guid,string>()))
                {
                    throw new CustomException(ResponseCodes.LAST_UPDATES_ALREADY_EXISTS,
                        ErrorMessages.DataBaseNoNeedUpdate);
                }

                var getDbFile = await GetDbFile( dbAddress, fileName);
                if (getDbFile.Data.Length == 0)
                    throw new CustomException(ResponseCodes.FILE_NOT_SAVED, ErrorMessages.FileIsCorruptErrorMessage);

                var parseFile =  FileService.ParseCsvDbFile(getDbFile.Data);
                if (parseFile == null || parseFile.Count == 0)
                    throw new CustomException(ResponseCodes.FAILURE, ErrorMessages.FileIsCorruptErrorMessage);

                var convertNetmaskRange =
                    NetMaskConverter.IpV4NetmaskRangeParse(parseFile.Select(s => s.Network).ToList());

                using (IDbService dbService = new DbService(_db).DbServiceInstance)
                {
                    var datesForSave = convertNetmaskRange.
                        Join(parseFile, ipRange => ipRange.NetMask, csv => csv.Network, (ipRange, csv) =>
                            new IpV4GeoLiteInformationEntity
                            {
                                Network = ipRange.NetMask,
                                Cidr = ipRange.Cidr,
                                AutonomousSystemNumber = csv.AutonomousSystemNumber,
                                AutonomousSystemOrganization = csv.AutonomousSystemOrganization,
                                MinFirstSegment = ipRange.MinFirstSegment,
                                MinSecondSegment = ipRange.MinSecondSegment,
                                MinThirdSegment = ipRange.MinThirdSegment,
                                MinLastSegment = ipRange.MinLastSegment,
                                MaxFirstSegment = ipRange.MaxFirstSegment,
                                MaxSecondSegment = ipRange.MaxSecondSegment,
                                MaxThirdSegment = ipRange.MaxThirdSegment,
                                MaxLastSegment = ipRange.MaxLastSegment,
                                Md5Sum = getLastHash.Data.Value,
                                HistoryKey = getLastHash.Data.Key
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