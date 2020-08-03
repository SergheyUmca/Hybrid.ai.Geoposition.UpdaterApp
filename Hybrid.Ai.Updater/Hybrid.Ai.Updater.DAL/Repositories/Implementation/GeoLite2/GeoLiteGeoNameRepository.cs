using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hybrid.Ai.Updater.Common.Models.Base;
using Hybrid.Ai.Updater.DAL.Context;
using Hybrid.Ai.Updater.DAL.Entities.GeoLite2;
using Hybrid.Ai.Updater.DAL.Models.Request.GeoLite;
using Hybrid.Ai.Updater.DAL.Models.Request.GeoLite.IpV4;
using Hybrid.Ai.Updater.DAL.Models.Response.GeoLite.IpV4;
using Hybrid.Ai.Updater.DAL.Repositories.Interfaces.GeoLite2;
using Microsoft.EntityFrameworkCore;
using ObjectCloner.Extensions;
using static Hybrid.Ai.Updater.Common.Models.Base.AppResponse;
using static Hybrid.Ai.Updater.Common.Models.Constants.ErrorMessages;

namespace Hybrid.Ai.Updater.DAL.Repositories.Implementation.GeoLite2
{
    public class GeoLiteGeoNameRepository : IGeoLiteGeoNameRepository
    {
        private readonly BaseContext _db;
        
        public GeoLiteGeoNameRepository(BaseContext context)
        {
            _db = context;
        }
        

        public async Task<Response<GeoNameBaseResponse>> Get(Guid historyKey, int geoNameId)
        {
            try
            {
                var result = await _db.GeoNameEntities
                    .Where(gn => gn.HistoryKey.Equals(historyKey) && gn.GeoNameId == geoNameId)
                    .Select(gn => new GeoNameBaseResponse
                    {
                        Key = gn.Key,
                        GeoNameId = gn.GeoNameId,
                        ContinentCode = gn.ContinentCode,
                        ContinentName = gn.ContinentName,
                        CountryIsoCode = gn.CountryIsoCode,
                        CountryName = gn.CountryName,
                        CityName = gn.CityName,
                        Subdivision1Name = gn.Subdivision1Name,
                        Subdivision1IsoCode = gn.Subdivision1IsoCode,
                        Subdivision2Name = gn.Subdivision2Name,
                        Subdivision2IsoCode = gn.Subdivision2IsoCode
                    }).AsNoTracking().FirstOrDefaultAsync();

                return result != null
                    ? new Response<GeoNameBaseResponse>(result.ShallowClone())
                    : new ErrorResponse<GeoNameBaseResponse>(NullSequenceErrorMessage, ResponseCodes.NOT_FOUND_RECORDS);
            }
            catch (CustomException e)
            {
                return new ErrorResponse<GeoNameBaseResponse>(e.Errors.FirstOrDefault()?.ResultMessage,
                    ResponseCodes.DATABASE_ERROR);
            }
            catch (Exception e)
            {
                var exceptionMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
                return new ErrorResponse<GeoNameBaseResponse>(exceptionMessage, ResponseCodes.DATABASE_ERROR);
            }
        }

        public async Task<Response<List<GeoNameBaseResponse>>> GetList(Guid historyKey)
        {
            try
            {
                var result = await _db.GeoNameEntities.Where(gn => gn.HistoryKey.Equals(historyKey))
                    .Select(gn => new GeoNameBaseResponse
                    {
                        Key = gn.Key,
                        GeoNameId = gn.GeoNameId,
                        ContinentCode = gn.ContinentCode,
                        ContinentName = gn.ContinentName,
                        CountryIsoCode = gn.CountryIsoCode,
                        CountryName = gn.CountryName,
                        CityName = gn.CityName,
                        Subdivision1Name = gn.Subdivision1Name,
                        Subdivision1IsoCode = gn.Subdivision1IsoCode,
                        Subdivision2Name = gn.Subdivision2Name,
                        Subdivision2IsoCode = gn.Subdivision2IsoCode
                    }).ToListAsync();

                return result != null ? new Response<List<GeoNameBaseResponse>>(result.ShallowClone()) 
                    : new ErrorResponse<List<GeoNameBaseResponse>>(NullSequenceErrorMessage,
                        ResponseCodes.NOT_FOUND_RECORDS);
            }
            catch (CustomException e)
            {
                return new ErrorResponse<List<GeoNameBaseResponse>>(e.Errors.FirstOrDefault()?.ResultMessage,
                    ResponseCodes.DATABASE_ERROR);
            }
            catch (Exception e)
            {
                var exceptionMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
                return new ErrorResponse<List<GeoNameBaseResponse>>(exceptionMessage, ResponseCodes.DATABASE_ERROR);
            }
        }

        public async Task<Response<List<GeoNameBaseResponse>>> CreateRange(List<GeoNameBaseRequest> directoryEntitiesList)
        {
            var clonedEntities = directoryEntitiesList.ShallowClone();
            
            var strategy = _db.Database.CreateExecutionStrategy();
            await strategy.Execute(async () =>
            {
                using (var transaction = await _db.Database.BeginTransactionAsync())
                {
                    var entitiesToBeCreated = new List<GeoNameEntity>();
                    try
                    {
                        foreach (var entity in clonedEntities) 
                            entity.Key = new Guid();

                        entitiesToBeCreated.AddRange(clonedEntities.Select(gn => new GeoNameEntity
                        {
                            Key = gn.Key,
                            GeoNameId = gn.GeoNameId,
                            ContinentCode = gn.ContinentCode,
                            ContinentName = gn.ContinentName,
                            CountryIsoCode = gn.CountryIsoCode,
                            CountryName = gn.CountryName,
                            CityName = gn.CityName,
                            Subdivision1Name = gn.Subdivision1Name,
                            Subdivision1IsoCode = gn.Subdivision1IsoCode,
                            Subdivision2Name = gn.Subdivision2Name,
                            Subdivision2IsoCode = gn.Subdivision2IsoCode,
                            HistoryKey = gn.HistoryKey
                        }));
                        
                        await _db.AddRangeAsync(entitiesToBeCreated);
                        await _db.SaveChangesAsync();
                        
                        await transaction.CommitAsync();

                        return new Response<List<GeoNameBaseResponse>>(clonedEntities.Select(gn =>
                            new GeoNameBaseResponse
                            {
                                Key = gn.Key,
                                GeoNameId = gn.GeoNameId,
                                ContinentCode = gn.ContinentCode,
                                ContinentName = gn.ContinentName,
                                CountryIsoCode = gn.CountryIsoCode,
                                CountryName = gn.CountryName,
                                CityName = gn.CityName,
                                Subdivision1Name = gn.Subdivision1Name,
                                Subdivision1IsoCode = gn.Subdivision1IsoCode,
                                Subdivision2Name = gn.Subdivision2Name,
                                Subdivision2IsoCode = gn.Subdivision2IsoCode
                            }).ToList());
                    }
                    catch (Exception e)
                    {
                        await transaction.RollbackAsync();
                        
                        var exceptionMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
                        return new ErrorResponse<List<GeoNameBaseResponse>>(exceptionMessage,
                            ResponseCodes.DATABASE_ERROR);
                    }
                }
            });
            return new Response<List<GeoNameBaseResponse>>(clonedEntities.Select(gn =>
                new GeoNameBaseResponse
                {
                    Key = gn.Key,
                    GeoNameId = gn.GeoNameId,
                    ContinentCode = gn.ContinentCode,
                    ContinentName = gn.ContinentName,
                    CountryIsoCode = gn.CountryIsoCode,
                    CountryName = gn.CountryName,
                    CityName = gn.CityName,
                    Subdivision1Name = gn.Subdivision1Name,
                    Subdivision1IsoCode = gn.Subdivision1IsoCode,
                    Subdivision2Name = gn.Subdivision2Name,
                    Subdivision2IsoCode = gn.Subdivision2IsoCode
                }).ToList());
        }

        public async Task<Response> Update(GeoNameBaseRequest entity)
        {
            try
            {
                var entityToDb = new GeoNameBaseResponse
                {
                    Key = entity.Key,
                    GeoNameId = entity.GeoNameId,
                    ContinentCode = entity.ContinentCode,
                    ContinentName = entity.ContinentName,
                    CountryIsoCode = entity.CountryIsoCode,
                    CountryName = entity.CountryName,
                    CityName = entity.CityName,
                    Subdivision1Name = entity.Subdivision1Name,
                    Subdivision1IsoCode = entity.Subdivision1IsoCode,
                    Subdivision2Name = entity.Subdivision2Name,
                    Subdivision2IsoCode = entity.Subdivision2IsoCode
                };
                
                var local = _db.Set<GeoNameEntity>().Local.FirstOrDefault(d => d.Key.Equals(entityToDb.Key));
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