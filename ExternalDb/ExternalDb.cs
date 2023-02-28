using ExternalDb.Entities;
using Microsoft.EntityFrameworkCore;

namespace ExternalDb
{
    public class ExternalDbContext: DbContext
    {
        public DbSet<ExternalCountry> ExternalCountries { get; set; }
        public DbSet<ExternalCity> ExternalCities { get; set; }
        public DbSet<ExternalOffice> ExternalOffices { get; set; }
        //public ExternalDbContext(DbContextOptions<ExternalDbContext> options)
        //    : base(options)
        //{
        //}

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data source = nba-091-01-UZ; Database= externalDb; Integrated Security=True;TrustServerCertificate=True");
            optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ExternalCountry>()
                .HasMany(c => c.Cities)
                .WithOne(ci => ci.Country)
                .HasForeignKey(ci => ci.CountryId)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<ExternalCity>()
                .HasMany(c => c.Offices)
                .WithOne(o => o.City)
                .HasForeignKey(o => o.CityId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}