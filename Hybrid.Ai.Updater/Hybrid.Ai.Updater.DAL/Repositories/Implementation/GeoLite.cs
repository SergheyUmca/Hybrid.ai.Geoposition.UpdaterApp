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
    public class GeoLite : IGeoLite
    {
        private readonly BaseContext _db;
        
        public GeoLite(BaseContext context)
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
        
        public async Task<Response<bool>> CheckHash(string md5Hash)
        {
            var vResult = new Response<bool>();
            try
            {
                var result = await _db.IpV4GeoLiteHistoryEntities.Where(w => w.Md5Sum.Equals(md5Hash) && w.Actualize)
                    .FirstOrDefaultAsync().ShallowClone();

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
                    await _db.IpV4GeoLiteHistoryEntities.AddAsync(new IpV4GeoLiteHistoryEntity
                    {
                        Actualize = true,
                        LastCheckDate = DateTime.Now,
                        Md5Sum = md5Hash,
                        UpdateDate = DateTime.Now
                    });
                    await _db.SaveChangesAsync();
                }

                vResult.Data = updateCheckInfo;
                
                return vResult;
            }
            catch (CustomException e)
            {
                return new ErrorResponse<bool>(e.Errors.FirstOrDefault()?.ResultMessage, ResponseCodes.DATABASE_ERROR);
            }
            catch (Exception e)
            {
                var exceptionMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
                return new ErrorResponse<bool>(exceptionMessage, ResponseCodes.DATABASE_ERROR);
            }
        }
        
        public async Task<Response<Dictionary<Guid, IpV4GeoLiteInformationEntity>>> CreateRange(List<IpV4GeoLiteInformationEntity> directoryEntitiesList)
        {
            try
            {
                await _db.IpV4GeoLiteInfoEntities.AddRangeAsync(directoryEntitiesList);
                await _db.SaveChangesAsync();
                var keyList = directoryEntitiesList.ToDictionary(directoryEntity => directoryEntity.Key);

                return new Response<Dictionary<Guid, IpV4GeoLiteInformationEntity>>(keyList.ShallowClone());
            }
            catch (Exception e)
            {
                var exceptionMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
                return new ErrorResponse<Dictionary<Guid, IpV4GeoLiteInformationEntity>>(exceptionMessage, ResponseCodes.DATABASE_ERROR);
            }
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