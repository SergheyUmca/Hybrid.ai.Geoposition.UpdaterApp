using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hybrid.Ai.Updater.Common.Models.Base;
using Hybrid.Ai.Updater.DAL.Context;
using Hybrid.Ai.Updater.DAL.Models.Response.GeoLite;
using Hybrid.Ai.Updater.DAL.Repositories.Interfaces.GeoLite2;
using Microsoft.EntityFrameworkCore;
using ObjectCloner.Extensions;
using static Hybrid.Ai.Updater.Common.Models.Base.AppResponse;
using static Hybrid.Ai.Updater.Common.Models.Constants.ErrorMessages;

namespace Hybrid.Ai.Updater.DAL.Repositories.Implementation.GeoLite2
{
    public class GeoLiteLanguageRepository : IGeoLiteLanguageRepository
    {
        private readonly BaseContext _db;
        
        public GeoLiteLanguageRepository(BaseContext context)
        {
            _db = context;
        }
        

        public async Task<Response<LanguageResponse>> Get(string code)
        {
            try
            {
                var result = await _db.LanguageEntities.Where(l => l.Code.Equals(code))
                    .Select(l => new LanguageResponse
                    {
                        Key = l.Key,
                        Code = l.Code,
                        Name = l.Name
                    }).AsNoTracking().FirstOrDefaultAsync();

                return result != null ? new Response<LanguageResponse>(result.ShallowClone()) 
                    : new ErrorResponse<LanguageResponse>(NullSequenceErrorMessage, ResponseCodes.NOT_FOUND_RECORDS);
            }
            catch (CustomException e)
            {
                return new ErrorResponse<LanguageResponse>(e.Errors.FirstOrDefault()?.ResultMessage, ResponseCodes.DATABASE_ERROR);
            }
            catch (Exception e)
            {
                var exceptionMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
                return new ErrorResponse<LanguageResponse>(exceptionMessage, ResponseCodes.DATABASE_ERROR);
            }
        }

        public async Task<Response<List<LanguageResponse>>> GetList()
        {
            try
            {
                var result = await _db.LanguageEntities
                    .Select(l => new LanguageResponse
                    {
                        Key = l.Key,
                        Code = l.Code,
                        Name = l.Name,
                    }).ToListAsync();

                return result != null ? new Response<List<LanguageResponse>>(result.ShallowClone()) 
                    : new ErrorResponse<List<LanguageResponse>>(NullSequenceErrorMessage, ResponseCodes.NOT_FOUND_RECORDS);
            }
            catch (CustomException e)
            {
                return new ErrorResponse<List<LanguageResponse>>(e.Errors.FirstOrDefault()?.ResultMessage, ResponseCodes.DATABASE_ERROR);
            }
            catch (Exception e)
            {
                var exceptionMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
                return new ErrorResponse<List<LanguageResponse>>(exceptionMessage, ResponseCodes.DATABASE_ERROR);
            }
        }
        
    }
}