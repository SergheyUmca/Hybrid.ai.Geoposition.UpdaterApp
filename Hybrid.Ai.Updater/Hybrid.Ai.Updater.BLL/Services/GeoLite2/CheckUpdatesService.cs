using System;
using System.Linq;
using System.Threading.Tasks;
using Hybrid.Ai.Updater.BLL.Models.Enums;
using Hybrid.Ai.Updater.BLL.Models.Service;
using Hybrid.Ai.Updater.Common.Models.Base;
using Hybrid.Ai.Updater.Common.Models.Constants;
using Hybrid.Ai.Updater.DAL.Context;
using Hybrid.Ai.Updater.DAL.Services.Implementation;
using Hybrid.Ai.Updater.DAL.Services.Interfaces;
using static Hybrid.Ai.Updater.Common.Models.Base.AppResponse;

namespace Hybrid.Ai.Updater.BLL.Services.GeoLite2
{
    public class CheckUpdatesService
    {
        private readonly BaseContext _db;

        public CheckUpdatesService(BaseContext context)
        {
            _db = context;
        }

        private const string AsnHashAddress =
            "https://download.maxmind.com/app/geoip_download?license_key=lsuPpqU6E5uiOSIS&suffix=zip.md5&edition_id=GeoLite2-ASN-CSV";
        private const string CityHashAddress =
            "https://download.maxmind.com/app/geoip_download?license_key=lsuPpqU6E5uiOSIS&suffix=zip.md5&edition_id=GeoLite2-City-CSV";
        
        public async Task<Response<CheckUpdateResponseModel>> CheckForUpdates(DataBaseTypes sourceType)
        {
            try
            {
                string getHashAddress;
                switch (sourceType)
                {
                    case DataBaseTypes.ASN:
                    {
                        getHashAddress = AsnHashAddress;
                        break;
                    }
                    case DataBaseTypes.CITY:
                    {
                        getHashAddress = CityHashAddress;
                        break;
                    }
                    default:
                    {
                        throw new CustomException(ResponseCodes.INTERNAL_BAD_REQUEST, ErrorMessages.WrongSourceType);
                    }
                }
                
                var getLastHash = await FileService.GetHash(getHashAddress);
                if (string.IsNullOrEmpty(getLastHash)) 
                    throw new CustomException(ResponseCodes.INTERNAL_BAD_REQUEST, ErrorMessages.BadExternalResponse);

                using (IDbService dbService = new DbService(_db).DbServiceInstance)
                {
                    var checkStoredHash = await dbService.GeoLiteHistory.CheckHash(getLastHash);
                    if (checkStoredHash.ResultCode != ResponseCodes.SUCCESS)
                    {
                        throw new CustomException(checkStoredHash.ResultCode,
                            checkStoredHash.Errors.FirstOrDefault()?.ResultMessage);
                    }

                    return new Response<CheckUpdateResponseModel>( new CheckUpdateResponseModel
                    {
                        HistoryRecordKey = checkStoredHash.Data.Key,
                        LastHashValue = checkStoredHash.Data.Md5Sum
                    });
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
    }
}