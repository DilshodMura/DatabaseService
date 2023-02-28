namespace DatabaseService
{
    public class DatabaseRepository
    {
        public static void SynchronizeData()
        {
            using (var sourceContext = new SourceDbContext())
            using (var targetContext = new TargetDbContext())
            {
                // Read data from the source database
                var sourceCountries = sourceContext.SourceCountries.ToList();
                var sourceCities = sourceContext.SourceCities.ToList();
                var sourceOffices = sourceContext.SourceOffices.ToList();

                // Transform data as needed
                var targetCountries = sourceCountries.Select(c => new TargetCountry { Id = c.Id, Name = c.Name }).ToList();
                var targetCities = sourceCities.Select(c => new TargetCity { Id = c.Id, Name = c.Name, CountryId = c.CountryId }).ToList();
                var targetOffices = sourceOffices.Select(o => new TargetOffice { Id = o.Id, Name = o.Name, CityId = o.CityId }).ToList();

                // Write data to the target database
                targetContext.Database.ExecuteSqlRaw("TRUNCATE TABLE TargetOffices");
                targetContext.TargetOffices.AddRange(targetOffices);
                targetContext.SaveChanges();

                targetContext.Database.ExecuteSqlRaw("TRUNCATE TABLE TargetCities");
                targetContext.TargetCities.AddRange(targetCities);
                targetContext.SaveChanges();

                targetContext.Database.ExecuteSqlRaw("TRUNCATE TABLE TargetCountries");
                targetContext.TargetCountries.AddRange(targetCountries);
                targetContext.SaveChanges();

            }
        }
    }
}
