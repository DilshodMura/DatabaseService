using InternalDb.Entities;
using Microsoft.EntityFrameworkCore;
using System;

namespace InternalDb
{
    public class InternalDbContext:DbContext
    {
        public DbSet<InternalCountry> InternalCountries { get; set; }
        public DbSet<InternalCity> InternalCities { get; set; }
        public DbSet<InternalOffice> InternalOffices { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data source = nba-091-01-UZ\\SQLEXPRESS; Database= internalDb; Integrated Security=True;TrustServerCertificate=True");
            optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<InternalCountry>()
                .HasMany(c => c.Cities)
                .WithOne(ci => ci.Country)
                .HasForeignKey(ci => ci.CountryId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<InternalCity>()
                .HasMany(c => c.Offices)
                .WithOne(o => o.City)
                .HasForeignKey(o => o.CityId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
