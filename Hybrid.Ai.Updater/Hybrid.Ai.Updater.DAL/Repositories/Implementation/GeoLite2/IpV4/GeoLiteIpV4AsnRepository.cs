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
    public class GeoLiteIpV4AsnRepository : IGeoLiteIpV4AsnRepository
    {
        private readonly BaseContext _db;
        
        public GeoLiteIpV4AsnRepository(BaseContext context)
        {
            _db = context;
        }
        

        public async Task<Response<Ipv4AsnBaseResponse>> Get(IpV4AsnSearchRequest searchRequest)
        {
            try
            {
                var result = await _db.GeoLiteIpV4AsnEntities.Where(w =>
                        (searchRequest.FirstSegment >= w.MinFirstSegment &&
                         searchRequest.FirstSegment <= w.MaxFirstSegment) &&
                        (searchRequest.SecondSegment >= w.MinSecondSegment &&
                         searchRequest.SecondSegment <= w.MaxSecondSegment) &&
                        (searchRequest.ThirdSegment >= w.MinThirdSegment &&
                         searchRequest.ThirdSegment <= w.MaxThirdSegment) &&
                        (searchRequest.LastSegment >= w.MinLastSegment &&
                         searchRequest.LastSegment <= w.MaxLastSegment))
                    .Join(_db.GeoLiteHistoryEntities, asn => asn.HistoryKey, history => history.Key, (asn, history) =>
                        new { asn, history})
                    .Where(w => w.history.Actualize == searchRequest.IsActualize)
                    .Select(je => new Ipv4AsnBaseResponse
                    {
                        Key = je.asn.Key,
                        AutonomousSystemNumber = je.asn.AutonomousSystemNumber,
                        AutonomousSystemOrganization = je.asn.AutonomousSystemOrganization,
                        Network = je.asn.Network,
                        Cidr = je.asn.Cidr,
                        Md5Sum = je.asn.Md5Sum,
                        HistoryKey = je.asn.HistoryKey,
                        MinFirstSegment = je.asn.MinFirstSegment,
                        MaxFirstSegment = je.asn.MaxFirstSegment,
                        MinSecondSegment = je.asn.MinSecondSegment,
                        MaxSecondSegment = je.asn.MaxSecondSegment,
                        MinThirdSegment = je.asn.MinThirdSegment,
                        MaxThirdSegment = je.asn.MaxThirdSegment,
                        MinLastSegment = je.asn.MinLastSegment,
                        MaxLastSegment = je.asn.MaxLastSegment,
                        IsActualize = je.history.Actualize
                    }).AsNoTracking().FirstOrDefaultAsync();

                return result != null ? new Response<Ipv4AsnBaseResponse>(result.ShallowClone()) 
                    : new ErrorResponse<Ipv4AsnBaseResponse>(NullSequenceErrorMessage, ResponseCodes.NOT_FOUND_RECORDS);
            }
            catch (CustomException e)
            {
                return new ErrorResponse<Ipv4AsnBaseResponse>(e.Errors.FirstOrDefault()?.ResultMessage,
                    ResponseCodes.DATABASE_ERROR);
            }
            catch (Exception e)
            {
                var exceptionMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
                return new ErrorResponse<Ipv4AsnBaseResponse>(exceptionMessage, ResponseCodes.DATABASE_ERROR);
            }
        }

        public async Task<Response<List<Ipv4AsnBaseResponse>>> GetList(Guid? historyKey = null)
        {
            try
            {
                var result = await _db.GeoLiteIpV4AsnEntities
                    .Where(asn => historyKey == null || asn.HistoryKey.Equals(historyKey))
                    .Join(_db.GeoLiteHistoryEntities, asn => asn.HistoryKey, history => history.Key, (asn, history) =>
                        new {asn, history})
                    .Select(je => new Ipv4AsnBaseResponse
                    {
                        Key = je.asn.Key,
                        AutonomousSystemNumber = je.asn.AutonomousSystemNumber,
                        AutonomousSystemOrganization = je.asn.AutonomousSystemOrganization,
                        Network = je.asn.Network,
                        Cidr = je.asn.Cidr,
                        Md5Sum = je.asn.Md5Sum,
                        HistoryKey = je.asn.HistoryKey,
                        MinFirstSegment = je.asn.MinFirstSegment,
                        MaxFirstSegment = je.asn.MaxFirstSegment,
                        MinSecondSegment = je.asn.MinSecondSegment,
                        MaxSecondSegment = je.asn.MaxSecondSegment,
                        MinThirdSegment = je.asn.MinThirdSegment,
                        MaxThirdSegment = je.asn.MaxThirdSegment,
                        MinLastSegment = je.asn.MinLastSegment,
                        MaxLastSegment = je.asn.MaxLastSegment,
                        IsActualize = je.history.Actualize
                    }).ToListAsync();

                return result != null ? new Response<List<Ipv4AsnBaseResponse>>(result.ShallowClone()) 
                    : new ErrorResponse<List<Ipv4AsnBaseResponse>>(NullSequenceErrorMessage,
                        ResponseCodes.NOT_FOUND_RECORDS);
            }
            catch (CustomException e)
            {
                return new ErrorResponse<List<Ipv4AsnBaseResponse>>(e.Errors.FirstOrDefault()?.ResultMessage,
                    ResponseCodes.DATABASE_ERROR);
            }
            catch (Exception e)
            {
                var exceptionMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
                return new ErrorResponse<List<Ipv4AsnBaseResponse>>(exceptionMessage, ResponseCodes.DATABASE_ERROR);
            }
        }

        public async Task<Response<List<Ipv4AsnBaseResponse>>> CreateRange(List<Ipv4AsnBaseRequest> entities)
        {
            var clonedEntities = entities.Select(asn => new Ipv4AsnBaseResponse
            {
                Key = asn.Key,
                AutonomousSystemNumber = asn.AutonomousSystemNumber,
                AutonomousSystemOrganization = asn.AutonomousSystemOrganization,
                Network = asn.Network,
                Cidr = asn.Cidr,
                Md5Sum = asn.Md5Sum,
                HistoryKey = asn.HistoryKey,
                MinFirstSegment = asn.MinFirstSegment,
                MaxFirstSegment = asn.MaxFirstSegment,
                MinSecondSegment = asn.MinSecondSegment,
                MaxSecondSegment = asn.MaxSecondSegment,
                MinThirdSegment = asn.MinThirdSegment,
                MaxThirdSegment = asn.MaxThirdSegment,
                MinLastSegment = asn.MinLastSegment,
                MaxLastSegment = asn.MaxLastSegment,
            }).ToList();
            
            var strategy = _db.Database.CreateExecutionStrategy();
            await strategy.Execute(async () =>
            {
                using (var transaction = await _db.Database.BeginTransactionAsync())
                {
                    // ReSharper disable once CollectionNeverQueried.Local
                    var entitiesToBeCreated = new List<GeoLiteIpV4AsnEntity>();
                    try
                    {
                        foreach (var entity in clonedEntities) 
                            entity.Key = new Guid();

                        entitiesToBeCreated.AddRange(clonedEntities.Select(entity => new GeoLiteIpV4AsnEntity
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
                        
                        await transaction.CommitAsync();

                        return new Response<List<Ipv4AsnBaseResponse>>(clonedEntities);
                    }
                    catch (Exception e)
                    {
                        await transaction.RollbackAsync();
                        
                        var exceptionMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
                        return new ErrorResponse<List<Ipv4AsnBaseResponse>>(exceptionMessage,
                            ResponseCodes.DATABASE_ERROR);
                    }
                }
            });
            return new Response<List<Ipv4AsnBaseResponse>>(clonedEntities);
        }

        public async Task<Response> Update(Ipv4AsnBaseRequest entity)
        {
            try
            {
                var entityToDb = new GeoLiteIpV4AsnEntity
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
                };
                
                var local = _db.Set<GeoLiteIpV4AsnEntity>().Local.FirstOrDefault(d => d.Key.Equals(entityToDb.Key));
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