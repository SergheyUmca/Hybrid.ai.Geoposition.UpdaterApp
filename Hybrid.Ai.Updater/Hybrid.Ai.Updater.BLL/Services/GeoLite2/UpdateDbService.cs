using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hybrid.Ai.Updater.BLL.Models.Enums;
using Hybrid.Ai.Updater.BLL.Models.Service;
using Hybrid.Ai.Updater.BLL.Models.Service.GeoLite2;
using Hybrid.Ai.Updater.Common.Models.Base;
using Hybrid.Ai.Updater.Common.Models.Constants;
using Hybrid.Ai.Updater.DAL.Context;
using Hybrid.Ai.Updater.DAL.Models.Request.GeoLite;
using Hybrid.Ai.Updater.DAL.Models.Request.GeoLite.IpV4;
using Hybrid.Ai.Updater.DAL.Services.Implementation;
using Hybrid.Ai.Updater.DAL.Services.Interfaces;
using static Hybrid.Ai.Updater.Common.Models.Base.AppResponse;

namespace Hybrid.Ai.Updater.BLL.Services.GeoLite2
{
    public class UpdateDbService
    {
        private readonly BaseContext _db;
        
        public UpdateDbService(BaseContext context)
        {
            _db = context;
        }
        
        public async Task<Response<bool>> SaveFilesToDb(DataBaseTypes sourceType, List<DbFileModel> dbFiles, 
        CheckUpdateResponseModel checkUpdate )
        {
            var response = new Response<bool>(false);
            try
            {
                switch (sourceType)
                {
                    case DataBaseTypes.ASN:
                    {
                        var saveAsnFile =
                            await SaveAsnFileToDb(dbFiles.FirstOrDefault(f => f.FileName.Contains("IPv4")),
                                checkUpdate);
                        if(saveAsnFile.ResultCode != ResponseCodes.SUCCESS)
                        {
                            throw new CustomException(saveAsnFile.ResultCode,
                                saveAsnFile.Errors.FirstOrDefault()?.ResultMessage);
                        }

                        break;
                    }
                    case DataBaseTypes.CITY:
                    {
                        if (dbFiles.Any(f => f.FileName.Contains("Locations") && f.FileName.Contains("City")))
                        {
                            var saveCityLocations = await SaveCityLocationsFileToDb(
                                dbFiles.Where(f => f.FileName.Contains("Locations") && f.FileName.Contains("City"))
                                    .ToList(), checkUpdate);
                            if(saveCityLocations.ResultCode != ResponseCodes.SUCCESS)
                            {
                                throw new CustomException(saveCityLocations.ResultCode,
                                    saveCityLocations.Errors.FirstOrDefault()?.ResultMessage);
                            }
                        }

                        var saveCityBlock = await SaveCityBlocksFileToDb(
                            dbFiles.FirstOrDefault(f =>
                                f.FileName.Contains("Blocks") && f.FileName.Contains("City") &&
                                f.FileName.Contains("IPv4")), checkUpdate);
                        if(saveCityBlock.ResultCode != ResponseCodes.SUCCESS)
                        {
                            throw new CustomException(saveCityBlock.ResultCode,
                                saveCityBlock.Errors.FirstOrDefault()?.ResultMessage);
                        }
                        
                        break;
                    }
                    default:
                    {
                        throw new CustomException(ResponseCodes.INVALID_PARAMETER,
                            ErrorMessages.InvalidRequestDataErrorMessage);
                    }
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

            return response;
        }

        private async Task<Response> SaveAsnFileToDb(DbFileModel file, CheckUpdateResponseModel checkUpdate)
        {
            try
            {
                if (checkUpdate.HistoryRecordKey == null) 
                    throw new CustomException(ResponseCodes.INVALID_PARAMETER, ErrorMessages.BadExternalResponse);

                var parseFile =  FileService.ParseCsvAsnDbFile(file.FileData);
                if (parseFile == null || parseFile.Count == 0)
                    throw new CustomException(ResponseCodes.FAILURE, ErrorMessages.FileIsCorruptErrorMessage);

                var convertNetmaskRange =
                    NetMaskConverter.IpV4NetmaskRangeParse(parseFile.Select(s => s.Network).ToList());

                using (IDbService dbService = new DbService(_db).DbServiceInstance)
                {
                    var datesForSave = convertNetmaskRange.
                        Join(parseFile, ipRange => ipRange.NetMask, csv => csv.Network, (ipRange, csv) =>
                            new Ipv4AsnBaseRequest
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
                                Md5Sum = checkUpdate.LastHashValue,
                                // ReSharper disable once PossibleInvalidOperationException
                                HistoryKey = (Guid)checkUpdate.HistoryRecordKey
                            }).ToList();
                
                    var checkStoredHash = await dbService.GeoLiteIpV4Asn.CreateRange(datesForSave);
                    if (checkStoredHash.ResultCode != ResponseCodes.SUCCESS)
                    {
                        throw new CustomException(checkStoredHash.ResultCode,
                            checkStoredHash.Errors.FirstOrDefault()?.ResultMessage);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
            return new Response();
        }
        
        private async Task<Response> SaveCityBlocksFileToDb(DbFileModel file, CheckUpdateResponseModel checkUpdate)
        {
            try
            {
                if (checkUpdate.HistoryRecordKey == null) 
                    throw new CustomException(ResponseCodes.INVALID_PARAMETER, ErrorMessages.BadExternalResponse);

                var parseFile =  FileService.ParseCsvCityBlocksDbFile(file.FileData);
                if (parseFile == null || parseFile.Count == 0)
                    throw new CustomException(ResponseCodes.FAILURE, ErrorMessages.FileIsCorruptErrorMessage);

                var convertNetmaskRange =
                    NetMaskConverter.IpV4NetmaskRangeParse(parseFile.Select(s => s.Network).ToList());

                using (IDbService dbService = new DbService(_db).DbServiceInstance)
                {
                    var datesForSave = convertNetmaskRange.
                        Join(parseFile, ipRange => ipRange.NetMask, csv => csv.Network, (ipRange, csv) =>
                            new CityBaseRequest
                            {
                                GeoNameId = csv.GeoNameId,
                                RegisteredCountryGeonameId = csv.RegisteredCountryGeonameId,
                                RepresentedCountryGeonameId = csv.RepresentedCountryGeonameId,
                                IsAnonymousProxy = csv.IsAnonymousProxy,
                                IsSatelliteProvider = csv.IsSatelliteProvider,
                                PostalCode = csv.PostalCode,
                                Latitude = csv.Latitude,
                                Longitude = csv.Longitude,
                                AccuracyRadius = csv.AccuracyRadius,
                                Network = ipRange.NetMask,
                                Cidr = ipRange.Cidr,
                                MinFirstSegment = ipRange.MinFirstSegment,
                                MinSecondSegment = ipRange.MinSecondSegment,
                                MinThirdSegment = ipRange.MinThirdSegment,
                                MinLastSegment = ipRange.MinLastSegment,
                                MaxFirstSegment = ipRange.MaxFirstSegment,
                                MaxSecondSegment = ipRange.MaxSecondSegment,
                                MaxThirdSegment = ipRange.MaxThirdSegment,
                                MaxLastSegment = ipRange.MaxLastSegment,
                                Md5Sum = checkUpdate.LastHashValue,
                                // ReSharper disable once PossibleInvalidOperationException
                                HistoryKey = (Guid)checkUpdate.HistoryRecordKey
                            }).ToList();
                
                    var checkStoredHash = await dbService.GeoLiteIpv4City.CreateRange(datesForSave);
                    if (checkStoredHash.ResultCode != ResponseCodes.SUCCESS)
                    {
                        throw new CustomException(checkStoredHash.ResultCode,
                            checkStoredHash.Errors.FirstOrDefault()?.ResultMessage);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
            return new Response();
        }
        
        private async Task<Response> SaveCityLocationsFileToDb(List<DbFileModel> files, CheckUpdateResponseModel checkUpdate)
        {
            try
            {
                if (checkUpdate.HistoryRecordKey == null) 
                    throw new CustomException(ResponseCodes.INVALID_PARAMETER, ErrorMessages.BadExternalResponse);

                var locations = new List<CsvParserCityLocations>();
                
                foreach (var parseFile in files.Select(file => FileService.ParseCsvCityLocationsDbFile(file.FileData)))
                {
                    if (parseFile == null || parseFile.Count == 0)
                        throw new CustomException(ResponseCodes.FAILURE, ErrorMessages.FileIsCorruptErrorMessage);
                    
                    locations.AddRange(parseFile);
                }
                

                using (IDbService dbService = new DbService(_db).DbServiceInstance)
                {
                    var datesForSave = locations.Select(csv => new GeoNameBaseRequest
                        {
                            GeoNameId = csv.GeoNameId,
                            ContinentCode = csv.ContinentCode,
                            ContinentName = csv.ContinentName,
                            CountryIsoCode = csv.CountryIsoCode,
                            CountryName = csv.CityName,
                            CityName = csv.CityName,
                            Subdivision1IsoCode = csv.Subdivision1IsoCode,
                            Subdivision1Name = csv.Subdivision1Name,
                            Subdivision2IsoCode = csv.Subdivision2IsoCode,
                            Subdivision2Name = csv.Subdivision2Name,
                            //Md5Sum = checkUpdate.LastHashValue,
                            // ReSharper disable once PossibleInvalidOperationException
                            HistoryKey = (Guid)checkUpdate.HistoryRecordKey
                        }).ToList();
                
                    var checkStoredHash = await dbService.GeoLiteGeoName.CreateRange(datesForSave);
                    if (checkStoredHash.ResultCode != ResponseCodes.SUCCESS)
                    {
                        throw new CustomException(checkStoredHash.ResultCode,
                            checkStoredHash.Errors.FirstOrDefault()?.ResultMessage);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
            return new Response();
        }
    }
}