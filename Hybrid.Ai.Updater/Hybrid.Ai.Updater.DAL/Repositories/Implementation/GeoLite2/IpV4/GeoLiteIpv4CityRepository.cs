using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hybrid.Ai.Updater.Common.Models.Base;
using Hybrid.Ai.Updater.DAL.Context;
using Hybrid.Ai.Updater.DAL.Entities.GeoLite2.IpV4;
using Hybrid.Ai.Updater.DAL.Models.Request.GeoLite.IpV4;
using Hybrid.Ai.Updater.DAL.Models.Response.GeoLite.IpV4;
using Hybrid.Ai.Updater.DAL.Repositories.Interfaces.GeoLite2.IpV4;
using Microsoft.EntityFrameworkCore;
using ObjectCloner.Extensions;
using static Hybrid.Ai.Updater.Common.Models.Base.AppResponse;
using static Hybrid.Ai.Updater.Common.Models.Constants.ErrorMessages;

namespace Hybrid.Ai.Updater.DAL.Repositories.Implementation.GeoLite2.IpV4
{
    public class GeoLiteIpv4CityRepository : IGeoLiteIpv4CityRepository
    {
        private readonly BaseContext _db;
        
        public GeoLiteIpv4CityRepository(BaseContext context)
        {
            _db = context;
        }
        
        

        public async Task<Response<IpV4CityBaseResponse>> Get(IpV4CitySearchRequest searchRequest)
        {
           try
           {
               var result = await _db.GeoLiteIpV4CityEntities.Where(w =>
                       (searchRequest.FirstSegment >= w.MinFirstSegment &&
                        searchRequest.FirstSegment <= w.MaxFirstSegment) &&
                       (searchRequest.SecondSegment >= w.MinSecondSegment &&
                        searchRequest.SecondSegment <= w.MaxSecondSegment) &&
                       (searchRequest.ThirdSegment >= w.MinThirdSegment &&
                        searchRequest.ThirdSegment <= w.MaxThirdSegment) &&
                       (searchRequest.LastSegment >= w.MinLastSegment &&
                        searchRequest.LastSegment <= w.MaxLastSegment))
                   .Join(_db.GeoLiteHistoryEntities, city => city.HistoryKey, history => history.Key, (city, history) =>
                       new { city, history})
                   .Where(w => w.history.Actualize == searchRequest.IsActualize)
                   .Select(je => new IpV4CityBaseResponse
                   {
                       Key = je.city.Key,
                       GeoNameId = je.city.GeoNameId,
                       RegisteredCountryGeonameId = je.city.RegisteredCountryGeonameId,
                       RepresentedCountryGeonameId = je.city.RepresentedCountryGeonameId,
                       IsAnonymousProxy = je.city.IsAnonymousProxy,
                       IsSatelliteProvider = je.city.IsSatelliteProvider,
                       PostalCode = je.city.PostalCode,
                       Latitude = je.city.Latitude,
                       Longitude = je.city.Latitude,
                       AccuracyRadius = je.city.AccuracyRadius,
                       Network = je.city.Network,
                       Cidr = je.city.Cidr,
                       Md5Sum = je.city.Md5Sum,
                       HistoryKey = je.city.HistoryKey,
                       MinFirstSegment = je.city.MinFirstSegment,
                       MaxFirstSegment = je.city.MaxFirstSegment,
                       MinSecondSegment = je.city.MinSecondSegment,
                       MaxSecondSegment = je.city.MaxSecondSegment,
                       MinThirdSegment = je.city.MinThirdSegment,
                       MaxThirdSegment = je.city.MaxThirdSegment,
                       MinLastSegment = je.city.MinLastSegment,
                       MaxLastSegment = je.city.MaxLastSegment,
                       IsActualize = je.history.Actualize
                   }).AsNoTracking().FirstOrDefaultAsync();

               return result != null ? new Response<IpV4CityBaseResponse>(result.ShallowClone()) 
                   : new ErrorResponse<IpV4CityBaseResponse>(NullSequenceErrorMessage, ResponseCodes.NOT_FOUND_RECORDS);
           }
           catch (CustomException e)
           {
               return new ErrorResponse<IpV4CityBaseResponse>(e.Errors.FirstOrDefault()?.ResultMessage,
                   ResponseCodes.DATABASE_ERROR);
           }
           catch (Exception e)
           {
               var exceptionMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
               return new ErrorResponse<IpV4CityBaseResponse>(exceptionMessage, ResponseCodes.DATABASE_ERROR);
           }
        }

        public async Task<Response<List<IpV4CityBaseResponse>>> GetList(bool isActualize = true)
        {
             try
             {
                 var result = await _db.GeoLiteIpV4CityEntities
                     .Join(_db.GeoLiteHistoryEntities, city => city.HistoryKey, history => history.Key,
                         (city, history) => new {city, history}).Where(je => je.history.Actualize == isActualize)
                     .Select(je => new IpV4CityBaseResponse
                     {
                         Key = je.city.Key,
                         GeoNameId = je.city.GeoNameId,
                         RegisteredCountryGeonameId = je.city.RegisteredCountryGeonameId,
                         RepresentedCountryGeonameId = je.city.RepresentedCountryGeonameId,
                         IsAnonymousProxy = je.city.IsAnonymousProxy,
                         IsSatelliteProvider = je.city.IsSatelliteProvider,
                         PostalCode = je.city.PostalCode,
                         Latitude = je.city.Latitude,
                         Longitude = je.city.Latitude,
                         AccuracyRadius = je.city.AccuracyRadius,
                         Network = je.city.Network,
                         Cidr = je.city.Cidr,
                         Md5Sum = je.city.Md5Sum,
                         HistoryKey = je.city.HistoryKey,
                         MinFirstSegment = je.city.MinFirstSegment,
                         MaxFirstSegment = je.city.MaxFirstSegment,
                         MinSecondSegment = je.city.MinSecondSegment,
                         MaxSecondSegment = je.city.MaxSecondSegment,
                         MinThirdSegment = je.city.MinThirdSegment,
                         MaxThirdSegment = je.city.MaxThirdSegment,
                         MinLastSegment = je.city.MinLastSegment,
                         MaxLastSegment = je.city.MaxLastSegment,
                         IsActualize = je.history.Actualize
                     }).ToListAsync();

                 return result != null ? new Response<List<IpV4CityBaseResponse>>(result.ShallowClone()) 
                     : new ErrorResponse<List<IpV4CityBaseResponse>>(NullSequenceErrorMessage,
                         ResponseCodes.NOT_FOUND_RECORDS);
             }
             catch (CustomException e)
             {
                 return new ErrorResponse<List<IpV4CityBaseResponse>>(e.Errors.FirstOrDefault()?.ResultMessage,
                     ResponseCodes.DATABASE_ERROR);
             }
             catch (Exception e)
             {
                 var exceptionMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
                 return new ErrorResponse<List<IpV4CityBaseResponse>>(exceptionMessage, ResponseCodes.DATABASE_ERROR);
             }
        }

        public async Task<Response<List<IpV4CityBaseResponse>>> CreateRange(List<CityBaseRequest> entities)
        {
           var clonedEntities = entities.Select(c => new IpV4CityBaseResponse
           {
               Key = c.Key,
               GeoNameId = c.GeoNameId,
               RegisteredCountryGeonameId = c.RegisteredCountryGeonameId,
               RepresentedCountryGeonameId = c.RepresentedCountryGeonameId,
               IsAnonymousProxy = c.IsAnonymousProxy,
               IsSatelliteProvider = c.IsSatelliteProvider,
               PostalCode = c.PostalCode,
               Latitude = c.Latitude,
               Longitude = c.Latitude,
               AccuracyRadius = c.AccuracyRadius,
               Network = c.Network,
               Cidr = c.Cidr,
               Md5Sum = c.Md5Sum,
               HistoryKey = c.HistoryKey,
               MinFirstSegment = c.MinFirstSegment,
               MaxFirstSegment = c.MaxFirstSegment,
               MinSecondSegment = c.MinSecondSegment,
               MaxSecondSegment = c.MaxSecondSegment,
               MinThirdSegment = c.MinThirdSegment,
               MaxThirdSegment = c.MaxThirdSegment,
               MinLastSegment = c.MinLastSegment,
               MaxLastSegment = c.MaxLastSegment,
           }).ToList();
            
            var strategy = _db.Database.CreateExecutionStrategy();
            await strategy.Execute(async () =>
            {
                using (var transaction = await _db.Database.BeginTransactionAsync())
                {
                    // ReSharper disable once CollectionNeverQueried.Local
                    var entitiesToBeCreated = new List<GeoLiteIpV4CityEntity>();
                    try
                    {
                        foreach (var entity in clonedEntities) 
                            entity.Key = new Guid();

                        entitiesToBeCreated.AddRange(clonedEntities.Select(c => new GeoLiteIpV4CityEntity
                        {
                            Key = c.Key,
                            GeoNameId = c.GeoNameId,
                            RegisteredCountryGeonameId = c.RegisteredCountryGeonameId,
                            RepresentedCountryGeonameId = c.RepresentedCountryGeonameId,
                            IsAnonymousProxy = c.IsAnonymousProxy,
                            IsSatelliteProvider = c.IsSatelliteProvider,
                            PostalCode = c.PostalCode,
                            Latitude = c.Latitude,
                            Longitude = c.Latitude,
                            AccuracyRadius = c.AccuracyRadius,
                            Network = c.Network,
                            Cidr = c.Cidr,
                            Md5Sum = c.Md5Sum,
                            HistoryKey = c.HistoryKey,
                            MinFirstSegment = c.MinFirstSegment,
                            MaxFirstSegment = c.MaxFirstSegment,
                            MinSecondSegment = c.MinSecondSegment,
                            MaxSecondSegment = c.MaxSecondSegment,
                            MinThirdSegment = c.MinThirdSegment,
                            MaxThirdSegment = c.MaxThirdSegment,
                            MinLastSegment = c.MinLastSegment,
                            MaxLastSegment = c.MaxLastSegment,
                        }));
                        
                        await _db.AddRangeAsync(entitiesToBeCreated);
                        await _db.SaveChangesAsync();
                        
                        await transaction.CommitAsync();

                        return new Response<List<IpV4CityBaseResponse>>(clonedEntities);
                    }
                    catch (Exception e)
                    {
                        await transaction.RollbackAsync();
                        
                        var exceptionMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
                        return new ErrorResponse<List<IpV4CityBaseResponse>>(exceptionMessage,
                            ResponseCodes.DATABASE_ERROR);
                    }
                }
            });
            return new Response<List<IpV4CityBaseResponse>>(clonedEntities);
        }

        public async Task<Response> Update(CityBaseRequest entity)
        {
            try
            {
                var entityToDb = new GeoLiteIpV4CityEntity
                {
                    Key = entity.Key,
                    GeoNameId = entity.GeoNameId,
                    RegisteredCountryGeonameId = entity.RegisteredCountryGeonameId,
                    RepresentedCountryGeonameId = entity.RepresentedCountryGeonameId,
                    IsAnonymousProxy = entity.IsAnonymousProxy,
                    IsSatelliteProvider = entity.IsSatelliteProvider,
                    PostalCode = entity.PostalCode,
                    Latitude = entity.Latitude,
                    Longitude = entity.Latitude,
                    AccuracyRadius = entity.AccuracyRadius,
                    Network = entity.Network,
                    Cidr = entity.Cidr,
                    Md5Sum = entity.Md5Sum,
                    HistoryKey = entity.HistoryKey,
                    MinFirstSegment = entity.MinFirstSegment,
                    MaxFirstSegment = entity.MaxFirstSegment,
                    MinSecondSegment = entity.MinSecondSegment,
                    MaxSecondSegment = entity.MaxSecondSegment,
                    MinThirdSegment = entity.MinThirdSegment,
                    MaxThirdSegment = entity.MaxThirdSegment,
                    MinLastSegment = entity.MinLastSegment,
                    MaxLastSegment = entity.MaxLastSegment,
                };
                
                var local = _db.Set<GeoLiteIpV4CityEntity>().Local.FirstOrDefault(d => d.Key.Equals(entityToDb.Key));
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