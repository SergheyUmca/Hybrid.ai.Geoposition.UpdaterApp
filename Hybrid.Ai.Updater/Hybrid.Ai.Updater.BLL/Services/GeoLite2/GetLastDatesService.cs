using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hybrid.Ai.Updater.BLL.Models.Enums;
using Hybrid.Ai.Updater.BLL.Models.Service.GeoLite2;
using Hybrid.Ai.Updater.Common.Models.Base;
using Hybrid.Ai.Updater.Common.Models.Constants;
using Hybrid.Ai.Updater.Common.Services;
using static Hybrid.Ai.Updater.Common.Models.Base.AppResponse;

namespace Hybrid.Ai.Updater.BLL.Services.GeoLite2
{
    public class GetLastDatesService
    {
        private const string AsnArchiveAddress =
            "https://download.maxmind.com/app/geoip_download?license_key=lsuPpqU6E5uiOSIS&suffix=zip&edition_id=GeoLite2-ASN-CSV";
        private const string CityArchiveAddress =
            "https://download.maxmind.com/app/geoip_download?license_key=lsuPpqU6E5uiOSIS&suffix=zip&edition_id=GeoLite2-City-CSV";
        
        public async Task<Response<List<DbFileModel>>> GetDbFile(DataBaseTypes sourceType)
        {
            var response = new Response<List<DbFileModel>>();
            try
            {
                string dbAddress;
                switch (sourceType)
                {
                    case DataBaseTypes.ASN:
                    {
                        dbAddress = AsnArchiveAddress;
                        break;
                    }
                    case DataBaseTypes.CITY:
                    {
                        dbAddress = CityArchiveAddress;
                        break;
                    }
                    default:
                    {
                        throw new CustomException(ResponseCodes.INTERNAL_BAD_REQUEST, ErrorMessages.WrongSourceType);
                    }
                }
                
                var getLastArchive = await FileService.DownloadDbFile(dbAddress);
                if (getLastArchive.Length == 0)
                {
                    response.Data = new List<DbFileModel>();
                    return response;
                }

                var unzipArchive = await ArchiveService.GetCsvFilesFromZip(getLastArchive);
                if (unzipArchive.Count > 0)
                {
                    response.Data = unzipArchive.Select(ua => new DbFileModel
                    {
                        FileData = ua.Data,
                        FileName = ua.Name
                    }).ToList();
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
    }
}