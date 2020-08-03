using Hybrid.Ai.Updater.DAL.Entities.GeoLite2;
using Hybrid.Ai.Updater.DAL.Entities.GeoLite2.IpV4;
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
            modelBuilder.Entity<GeoLiteIpV4AsnEntity>().HasKey(e => e.Key);
            
            modelBuilder.Entity<GeoLiteHistoryEntity>().HasKey(e => e.Key);
            
            modelBuilder.Entity<GeoNameEntity>().HasKey(e => e.Key);
            
            modelBuilder.Entity<GeoLiteIpV4CityEntity>().HasKey(e => e.Key);
            
            modelBuilder.Entity<LanguageEntity>().HasKey(e => e.Key);
            
            
            // modelBuilder.Entity<OptionDeclensionsEntity>(entity =>
            // {
            //     // OptionDeclensionsEntity primary Key.
            //     entity.HasKey(o => o.Key);
            //     entity.Property(o => o.Key).ValueGeneratedOnAdd().Metadata
            //         .BeforeSaveBehavior = PropertySaveBehavior.Ignore;
            //     
            //     //OptionDeclensionsEntity add checkConstraint
            //     entity.Property(o => o.OptionKey).IsRequired();
            //     entity.Property(o => o.DeclensionKey).IsRequired();
            //
            //     //OptionDeclensionsEntity add link Many to Many
            //     entity.HasOne(od => od.Declension)
            //         .WithMany(d => d.OptionDeclensions)
            //         .HasForeignKey(od => od.DeclensionKey);
            //     
            //     //OptionDeclensionsEntity add link Many to Many
            //     entity.HasOne(od => od.Option)
            //         .WithMany(o => o.OptionDeclensions)
            //         .HasForeignKey(od => od.OptionKey);
            // });
        }
        
        
        public DbSet<GeoLiteIpV4CityEntity> GeoLiteIpV4CityEntities { get; set; }
        
        public DbSet<GeoLiteIpV4AsnEntity> GeoLiteIpV4AsnEntities { get; set; }
        
        public DbSet<GeoLiteHistoryEntity> GeoLiteHistoryEntities { get; set; }
        
        public DbSet<GeoNameEntity> GeoNameEntities { get; set; }
        
        public DbSet<LanguageEntity> LanguageEntities { get; set; }
    }
}