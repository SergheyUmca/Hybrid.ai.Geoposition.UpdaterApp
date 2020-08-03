using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hybrid.Ai.Updater.Common.Models.Base;
using Hybrid.Ai.Updater.DAL.Context;
using Hybrid.Ai.Updater.DAL.Entities.GeoLite2;
using Hybrid.Ai.Updater.DAL.Models.Request.GeoLite;
using Hybrid.Ai.Updater.DAL.Models.Response.GeoLite;
using Hybrid.Ai.Updater.DAL.Repositories.Interfaces.GeoLite2;
using Microsoft.EntityFrameworkCore;
using ObjectCloner.Extensions;
using static Hybrid.Ai.Updater.Common.Models.Base.AppResponse;
using static Hybrid.Ai.Updater.Common.Models.Constants.ErrorMessages;

namespace Hybrid.Ai.Updater.DAL.Repositories.Implementation.GeoLite2
{
    public class GeoLiteHistoryRepository : IGeoLiteHistoryRepository
    {
        private readonly BaseContext _db;
        
        public GeoLiteHistoryRepository(BaseContext context)
        {
            _db = context;
        }

        public async Task<Response<HistoryBaseResponse>> Get(Guid key)
        {
            try
            {
                var result = await _db.GeoLiteHistoryEntities.Where(he => he.Key.Equals(key))
                    .Select(he => new HistoryBaseResponse
                    {
                        Key = he.Key,
                        Md5Sum = he.Md5Sum,
                        LastCheckDate = he.LastCheckDate,
                        Actualize = he.Actualize,
                        UpdateDate = he.UpdateDate
                    }).AsNoTracking().FirstOrDefaultAsync();

                return result != null ? new Response<HistoryBaseResponse>(result.ShallowClone()) 
                    : new ErrorResponse<HistoryBaseResponse>(NullSequenceErrorMessage, ResponseCodes.NOT_FOUND_RECORDS);
            }
            catch (CustomException e)
            {
                return new ErrorResponse<HistoryBaseResponse>(e.Errors.FirstOrDefault()?.ResultMessage,
                    ResponseCodes.DATABASE_ERROR);
            }
            catch (Exception e)
            {
                var exceptionMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
                return new ErrorResponse<HistoryBaseResponse>(exceptionMessage, ResponseCodes.DATABASE_ERROR);
            }
        }

        public async Task<Response<List<HistoryBaseResponse>>> GetList(bool isLast = false)
        {
            try
            {
                var result = await _db.GeoLiteHistoryEntities.Where(ge => ge.Actualize)
                    .Select(he => new HistoryBaseResponse
                    {
                        Key = he.Key,
                        Md5Sum = he.Md5Sum,
                        LastCheckDate = he.LastCheckDate,
                        Actualize = he.Actualize,
                        UpdateDate = he.UpdateDate
                    }).ToListAsync();

                return result != null ? new Response<List<HistoryBaseResponse>>(result.ShallowClone()) 
                    : new ErrorResponse<List<HistoryBaseResponse>>(NullSequenceErrorMessage,
                        ResponseCodes.NOT_FOUND_RECORDS);
            }
            catch (CustomException e)
            {
                return new ErrorResponse<List<HistoryBaseResponse>>(e.Errors.FirstOrDefault()?.ResultMessage,
                    ResponseCodes.DATABASE_ERROR);
            }
            catch (Exception e)
            {
                var exceptionMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
                return new ErrorResponse<List<HistoryBaseResponse>>(exceptionMessage, ResponseCodes.DATABASE_ERROR);
            }
        }

        async Task<Response<HistoryBaseResponse>> IGeoLiteHistoryRepository.CheckHash(string md5Hash)
        {
            var vResult = new Response<HistoryBaseResponse>();
            try
            {
                var getHistoryEntity = await _db.GeoLiteHistoryEntities.Where(w => w.Md5Sum.Equals(md5Hash) && w.Actualize)
                    .AsNoTracking().FirstOrDefaultAsync();

                var updateCheckInfo = getHistoryEntity != null;
                if (updateCheckInfo)
                {
                    getHistoryEntity.LastCheckDate = DateTime.Now;
                    
                    var local = _db.Set<GeoLiteHistoryEntity>().Local
                        .FirstOrDefault(d => d.Key.Equals(getHistoryEntity.Key));
                    
                    if (local != null)
                        _db.Entry(local).State = EntityState.Detached;

                    _db.Entry(getHistoryEntity).State = EntityState.Modified;
                    await _db.SaveChangesAsync();
                    
                    vResult.Data = new HistoryBaseResponse
                    {
                        Key = getHistoryEntity.Key,
                        Md5Sum = getHistoryEntity.Md5Sum,
                        LastCheckDate = getHistoryEntity.LastCheckDate,
                        Actualize = getHistoryEntity.Actualize,
                        UpdateDate = getHistoryEntity.UpdateDate
                    };
                }
                else
                {
                    var historyEntity = new GeoLiteHistoryEntity
                    {
                        Key = Guid.NewGuid(),
                        Actualize = true,
                        LastCheckDate = DateTime.Now,
                        Md5Sum = md5Hash,
                        UpdateDate = DateTime.Now
                    };
                    
                    await _db.GeoLiteHistoryEntities.AddAsync(historyEntity);
                    await _db.SaveChangesAsync();
                    
                    vResult.Data = new HistoryBaseResponse
                    {
                        Key = historyEntity.Key,
                        Md5Sum = historyEntity.Md5Sum,
                        LastCheckDate = historyEntity.LastCheckDate,
                        Actualize = historyEntity.Actualize,
                        UpdateDate = historyEntity.UpdateDate
                    };
                }
                
                return vResult;
            }
            catch (CustomException e)
            {
                return new ErrorResponse<HistoryBaseResponse>(e.Errors.FirstOrDefault()?.ResultMessage,
                    ResponseCodes.DATABASE_ERROR);
            }
            catch (Exception e)
            {
                var exceptionMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
                return new ErrorResponse<HistoryBaseResponse>(exceptionMessage, ResponseCodes.DATABASE_ERROR);
            }
        }
        

        public async Task<Response> Update(HistoryBaseRequest entity)
        {
            try
            {
                var entityToDb = new GeoLiteHistoryEntity
                {
                    Key = entity.Key,
                    Actualize = entity.Actualize,
                    LastCheckDate = entity.LastCheckDate,
                    Md5Sum = entity.Md5Sum,
                    UpdateDate = DateTime.Now
                };
                var local = _db.Set<GeoLiteHistoryEntity>().Local.FirstOrDefault(d => d.Key.Equals(entityToDb.Key));
                if (local != null)
                    _db.Entry(local).State = EntityState.Detached;
              
                _db.Entry(entityToDb).State = EntityState.Modified;
                await _db.SaveChangesAsync();
                return new Response();
            }
            catch (Exception e)
            {
                var exceptionMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
                return new ErrorResponse(exceptionMessage, ResponseCodes.DATABASE_ERROR);
            }
        }
        
    }
}