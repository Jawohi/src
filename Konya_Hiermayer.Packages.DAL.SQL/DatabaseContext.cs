using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Konya_Hiermayer.Packages.DAL.Entities;

namespace Konya_Hiermayer.Packages.DAL.SQL
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Hop> Hops { get; set; }
        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<Parcel> Parcels { get; set; }
        public DbSet<Webhook> Webhooks { get; set; }

        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
            //this.Database.EnsureDeleted();
            this.Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //Trusted_Connection=True;
            optionsBuilder.UseSqlServer(
                @"Server=localhost,1433;
                Database=sks;
                User=SA;
                Password=pass@word1;
                "
                );
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Warehouse>();
            modelBuilder.Entity<Truck>();
            modelBuilder.Entity<Transferwarehouse>();
            modelBuilder.Entity<WarehouseNextHops>();
            modelBuilder.Entity<Parcel>();

            modelBuilder.Entity<Hop>().HasKey(h => h.Code);
            modelBuilder.Entity<Parcel>().HasKey(p => p.TrackingId);
            modelBuilder.Entity<HopArrival>().HasKey(ha => new { ha.HopId, ha.ParcelId });
            modelBuilder.Entity<WarehouseNextHops>().HasKey(ha => new { ha.FirstHopId, ha.SecondHopId });

            modelBuilder.Entity<Hop>().ToTable("hop")
                 .HasDiscriminator<int>("HopType")
                 .HasValue<Warehouse>(1)
                 .HasValue<Truck>(2)
                 .HasValue<Transferwarehouse>(3);

            modelBuilder.Entity<Truck>().Property(t => t.RegionGeoJson).HasColumnType("geometry");
            modelBuilder.Entity<Transferwarehouse>().Property(t => t.RegionGeoJson).HasColumnType("geometry");

            modelBuilder.Entity<HopArrival>()
                .HasOne(hp => hp.Parcel)
                .WithMany(p => p.Hops)
                .HasForeignKey(hp => hp.ParcelId);

            modelBuilder.Entity<HopArrival>()
                .HasOne(h => h.Hop)
                .WithMany()
                .HasForeignKey(h => h.HopId);

            modelBuilder.Entity<Warehouse>()
                .HasMany(w => w.NextHops)
                .WithOne(nh => nh.FirstHop);

            modelBuilder.Entity<WarehouseNextHops>()
                .HasOne(wnh => wnh.FirstHop)
                .WithMany(w => w.NextHops)
                .HasForeignKey(wnh => wnh.FirstHopId).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<WarehouseNextHops>()
                .HasOne(wnh => wnh.SecondHop)
                .WithMany()
                .HasForeignKey(wnh => wnh.SecondHopId).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Parcel>()
               .HasMany(p => p.Hops)
               .WithOne(ha => ha.Parcel);

            modelBuilder.Entity<Parcel>()
                .OwnsOne(p => p.Recipient);

            modelBuilder.Entity<Parcel>()
                .OwnsOne(p => p.Sender);

            modelBuilder.Entity<Webhook>()
                .HasKey(k => k.Id);
            modelBuilder.Entity<Webhook>()
                .Property(p => p.Id)
                .ValueGeneratedOnAdd();


            base.OnModelCreating(modelBuilder);
        }
    }
}
