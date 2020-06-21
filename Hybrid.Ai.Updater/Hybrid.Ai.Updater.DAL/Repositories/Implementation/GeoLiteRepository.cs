using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hybrid.ai.Geoposition.Common.Models.BaseModels;
using Hybrid.Ai.Updater.DAL.Context;
using Hybrid.Ai.Updater.DAL.Entities;
using Hybrid.Ai.Updater.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using ObjectCloner.Extensions;
using static Hybrid.ai.Geoposition.Common.Models.BaseModels.Response;
using static Hybrid.ai.Geoposition.Common.Models.BaseModels.Response.AppResponse;
using static Hybrid.Ai.Updater.Common.Models.Constants.ErrorMessages;

namespace Hybrid.Ai.Updater.DAL.Repositories.Implementation
{
    public class GeoLiteRepository : IGeoLiteRepository
    {
        private readonly BaseContext _db;
        
        public GeoLiteRepository(BaseContext context)
        {
            _db = context;
        }
        
        
        //Todo add search by network
        public async Task<Response<IpV4GeoLiteInformationEntity>> Get(string md5Hash, string network)
        {
            try
            {
                var result = await _db.IpV4GeoLiteInfoEntities.Where(w => w.Md5Sum.Equals(md5Hash))
                    .Select(s => new IpV4GeoLiteInformationEntity
                    {
                        Key = s.Key,
                        AutonomousSystemNumber = s.AutonomousSystemNumber,
                        AutonomousSystemOrganization = s.AutonomousSystemOrganization,
                        Network = s.Network,
                        Md5Sum = s.Md5Sum
                    }).AsNoTracking().FirstOrDefaultAsync();

                return result != null ? new Response<IpV4GeoLiteInformationEntity>(result.ShallowClone()) 
                    : new ErrorResponse<IpV4GeoLiteInformationEntity>(NullSequenceErrorMessage, ResponseCodes.NOT_FOUND_RECORDS);
            }
            catch (CustomException e)
            {
                return new ErrorResponse<IpV4GeoLiteInformationEntity>(e.Errors.FirstOrDefault()?.ResultMessage, ResponseCodes.DATABASE_ERROR);
            }
            catch (Exception e)
            {
                var exceptionMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
                return new ErrorResponse<IpV4GeoLiteInformationEntity>(exceptionMessage, ResponseCodes.DATABASE_ERROR);
            }
        }

        public async Task<Response<List<IpV4GeoLiteInformationEntity>>> GetList(string md5Hash)
        {
            try
            {
                var result = await _db.IpV4GeoLiteInfoEntities.Where(w => w.Md5Sum.Equals(md5Hash))
                    .Join(_db.IpV4GeoLiteHistoryEntities, info => info.Md5Sum, history => history.Md5Sum, (info, history) =>  new
                    {
                        info,
                        history
                    })
                    .Where(w => w.history.Actualize)
                    .Select(s => new IpV4GeoLiteInformationEntity
                    {
                        Key = s.info.Key,
                        AutonomousSystemNumber = s.info.AutonomousSystemNumber,
                        AutonomousSystemOrganization = s.info.AutonomousSystemOrganization,
                        Network = s.info.Network,
                        Md5Sum = s.info.Md5Sum
                    }).ToListAsync();

                return result != null ? new Response<List<IpV4GeoLiteInformationEntity>>(result.ShallowClone()) 
                    : new ErrorResponse<List<IpV4GeoLiteInformationEntity>>(NullSequenceErrorMessage, ResponseCodes.NOT_FOUND_RECORDS);
            }
            catch (CustomException e)
            {
                return new ErrorResponse<List<IpV4GeoLiteInformationEntity>>(e.Errors.FirstOrDefault()?.ResultMessage, ResponseCodes.DATABASE_ERROR);
            }
            catch (Exception e)
            {
                var exceptionMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
                return new ErrorResponse<List<IpV4GeoLiteInformationEntity>>(exceptionMessage, ResponseCodes.DATABASE_ERROR);
            }
        }
        
        public async Task<Response<Guid?>> CheckHash(string md5Hash)
        {
            var vResult = new Response<Guid?>();
            try
            {
                var result = await _db.IpV4GeoLiteHistoryEntities.Where(w => w.Md5Sum.Equals(md5Hash) && w.Actualize)
                    .FirstOrDefaultAsync();

                var updateCheckInfo = result != null;
                if (updateCheckInfo)
                {
                    result.LastCheckDate = DateTime.Now;
                    var local = _db.Set<IpV4GeoLiteHistoryEntity>().Local.FirstOrDefault(d => d.Key.Equals(result.Key));
                    if (local != null)
                        _db.Entry(local).State = EntityState.Detached;

                    _db.Entry(result).State = EntityState.Modified;
                    await _db.SaveChangesAsync();
                }
                else
                {
                    var historyEntity = new IpV4GeoLiteHistoryEntity
                    {
                        Key = Guid.NewGuid(),
                        Actualize = true,
                        LastCheckDate = DateTime.Now,
                        Md5Sum = md5Hash,
                        UpdateDate = DateTime.Now
                    };
                    
                    await _db.IpV4GeoLiteHistoryEntities.AddAsync(historyEntity);
                    await _db.SaveChangesAsync();
                    
                    vResult.Data = historyEntity.Key;
                }
                
                return vResult;
            }
            catch (CustomException e)
            {
                return new ErrorResponse<Guid?>(e.Errors.FirstOrDefault()?.ResultMessage, ResponseCodes.DATABASE_ERROR);
            }
            catch (Exception e)
            {
                var exceptionMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
                return new ErrorResponse<Guid?>(exceptionMessage, ResponseCodes.DATABASE_ERROR);
            }
        }

        public async Task<Response<List<IpV4GeoLiteInformationEntity>>> CreateRange(
            List<IpV4GeoLiteInformationEntity> directoryEntitiesList)
        {
            var clonedEntities = directoryEntitiesList.ShallowClone();
            
            var strategy = _db.Database.CreateExecutionStrategy();
            await strategy.Execute(async () =>
            {
                using (var transaction = _db.Database.BeginTransaction())
                {
                    // ReSharper disable once CollectionNeverQueried.Local
                    var entitiesToBeCreated = new List<IpV4GeoLiteInformationEntity>();
                    try
                    {
                        foreach (var entity in clonedEntities) 
                            entity.Key = new Guid();

                        entitiesToBeCreated.AddRange(clonedEntities.Select(entity => new IpV4GeoLiteInformationEntity
                        {
                            Key = entity.Key,
                            Network = entity.Network,
                            Md5Sum = entity.Md5Sum,
                            HistoryKey = entity.HistoryKey,
                            AutonomousSystemNumber = entity.AutonomousSystemNumber,
                            AutonomousSystemOrganization = entity.AutonomousSystemOrganization,
                            Cidr = entity.Cidr,
                            MinFirstSegment = entity.MinFirstSegment,
                            MinSecondSegment = entity.MinSecondSegment,
                            MinThirdSegment = entity.MinThirdSegment,
                            MinLastSegment = entity.MinLastSegment,
                            MaxFirstSegment = entity.MaxFirstSegment,
                            MaxSecondSegment = entity.MaxSecondSegment,
                            MaxThirdSegment = entity.MaxThirdSegment,
                            MaxLastSegment = entity.MaxLastSegment
                        }));
                        
                        await _db.AddRangeAsync(entitiesToBeCreated);
                        await _db.SaveChangesAsync();
                        
                        transaction.Commit();

                        return new Response<List<IpV4GeoLiteInformationEntity>>(clonedEntities);
                    }
                    catch (Exception e)
                    {
                        transaction.Rollback();
                        
                        var exceptionMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
                        return new ErrorResponse<List<IpV4GeoLiteInformationEntity>>(exceptionMessage,
                            ResponseCodes.DATABASE_ERROR);
                    }
                }
            });
            return new Response<List<IpV4GeoLiteInformationEntity>>(clonedEntities);
        }

        public async Task<AppResponse.Response> Update(IpV4GeoLiteInformationEntity entity)
        {
            try
            {
                var entityToDb = entity.ShallowClone();
                var local = _db.Set<IpV4GeoLiteInformationEntity>().Local.FirstOrDefault(d => d.Key.Equals(entityToDb.Key));
                if (local != null)
                    _db.Entry(local).State = EntityState.Detached;
              
                _db.Entry(entityToDb).State = EntityState.Modified;
                await _db.SaveChangesAsync();
                return new AppResponse.Response();
            }
            catch (Exception e)
            {
                var exceptionMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
                return new ErrorResponse(exceptionMessage, ResponseCodes.DATABASE_ERROR);
            }
        }
    }
}