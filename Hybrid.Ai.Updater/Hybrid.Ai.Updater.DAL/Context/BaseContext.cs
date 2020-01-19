using Hybrid.Ai.Updater.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace Hybrid.Ai.Updater.DAL.Context
{
    public sealed class BaseContext : DbContext
    {
        public BaseContext(DbContextOptions options) : base(options)
        {
            Database.EnsureCreated();
        }

        // All keys and constraints are performed here.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // EntityStringEntity primary key.
            modelBuilder.Entity<IpV4GeoLiteInformationEntity>().HasKey(e => e.Key);
            
            modelBuilder.Entity<IpV4GeoLiteHistoryEntity>().HasKey(e => e.Key);
        }
        
        public DbSet<IpV4GeoLiteInformationEntity> IpV4GeoLiteInfoEntities { get; set; }
        public DbSet<IpV4GeoLiteHistoryEntity> IpV4GeoLiteHistoryEntities { get; set; }
    }
}