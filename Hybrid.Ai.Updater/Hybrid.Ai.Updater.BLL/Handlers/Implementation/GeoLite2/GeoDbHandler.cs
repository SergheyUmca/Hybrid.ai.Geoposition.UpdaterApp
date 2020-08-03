using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hybrid.Ai.Updater.BLL.Handlers.Interfaces.GeoLite2;
using Hybrid.Ai.Updater.BLL.Models.Enums;
using Hybrid.Ai.Updater.BLL.Models.Service;
using Hybrid.Ai.Updater.BLL.Models.Service.GeoLite2;
using Hybrid.Ai.Updater.BLL.Services.GeoLite2;
using Hybrid.Ai.Updater.Common.Models.Base;
using Hybrid.Ai.Updater.Common.Models.Constants;
using Hybrid.Ai.Updater.DAL.Context;
using static Hybrid.Ai.Updater.Common.Models.Base.AppResponse;

namespace Hybrid.Ai.Updater.BLL.Handlers.Implementation.GeoLite2
{
    public class GeoDbHandler : IGeoDbHandler
    {
        private static CheckUpdatesService _checkUpdates;
        private static GetLastDatesService _lastDatesService;
        private static UpdateDbService _updateDbService;

        public GeoDbHandler(BaseContext context)
        {
            _checkUpdates = new CheckUpdatesService(context);
            _lastDatesService = new GetLastDatesService();
            _updateDbService = new UpdateDbService(context);
        }
        
        // ReSharper disable once MemberCanBePrivate.Global
        public async Task<Response<CheckUpdateResponseModel>> CheckForUpdates(DataBaseTypes sourceType)
        {
            try
            {
                var checkUpdates = await _checkUpdates.CheckForUpdates(sourceType);
                if (checkUpdates.ResultCode != ResponseCodes.SUCCESS)
                {
                    throw new CustomException(checkUpdates.ResultCode,
                        checkUpdates.Errors.FirstOrDefault()?.ResultMessage);
                }
                
                return new Response<CheckUpdateResponseModel>(checkUpdates.Data);
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
        
        public async Task<Response<List<DbFileModel>>> GetDbFile(DataBaseTypes sourceType)
        {
            var response = new Response<List<DbFileModel>>();
            try
            {
                var getDbFiles = await _lastDatesService.GetDbFile(sourceType);
                if(getDbFiles.ResultCode != ResponseCodes.SUCCESS)
                {
                    throw new CustomException(getDbFiles.ResultCode,
                        getDbFiles.Errors.FirstOrDefault()?.ResultMessage);
                }

                response.Data = getDbFiles.Data;
                return response;
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

        public async Task<Response<bool>> UpdateDb(DataBaseTypes sourceType)
        {
            var vResult = new Response<bool>(false);
            try
            {
                //TODO add logic for LAST_UPDATES_ALREADY_EXISTS
                var getLastHash = await CheckForUpdates(sourceType);
                if (getLastHash.Data?.HistoryRecordKey == null)
                {
                    throw new CustomException(ResponseCodes.LAST_UPDATES_ALREADY_EXISTS,
                        ErrorMessages.DataBaseNoNeedUpdate);
                }

                var getFileFromExternalService = await GetDbFile(sourceType);
                if (getFileFromExternalService.Data?.Count < 1)
                    throw new CustomException(ResponseCodes.FILE_NOT_SAVED, ErrorMessages.FileIsCorruptErrorMessage);

                var updateDb =
                    await _updateDbService.SaveFilesToDb(sourceType, getFileFromExternalService.Data, getLastHash.Data);
                if(updateDb.ResultCode != ResponseCodes.SUCCESS)
                {
                    throw new CustomException(ResponseCodes.FILE_NOT_SAVED,
                        ErrorMessages.InsertNewRowsIntoDbErrorMessage);
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