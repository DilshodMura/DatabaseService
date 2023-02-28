using ExternalDb;
using ExternalDb.Entities;
using InternalDb;
using InternalDb.Entities;
using Microsoft.EntityFrameworkCore;

namespace DbService
{
    public class SyncService:BackgroundService
    {
        private readonly ILogger<SyncService> _logger;

        public SyncService(ILogger<SyncService> logger)
        {
            _logger = logger;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Starting synchronization...");

                    // Synchronize the data in the Country table
                    await SyncCountriesAsync();

                    // Synchronize the data in the City table
                    await SyncCitiesAsync();

                    // Synchronize the data in the Office table
                    await SyncOfficesAsync();

                    _logger.LogInformation("Synchronization complete.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during synchronization.");
                }

                // Wait for the specified interval before running the next synchronization
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }

        private async Task SyncCountriesAsync()
        {
            await using var _externalDbContext = new ExternalDbContext();
            await using var _dbContext = new InternalDbContext();
            // Read the data from the internal database
            var internalCountries = await _dbContext.InternalCountries.ToListAsync();

            // Read the data from the external database
            var externalCountries = await _externalDbContext.ExternalCountries.ToListAsync();

            // Identify the differences between the two sets of data
            var internalIds = internalCountries.Select(c => c.Id).ToHashSet();
            var externalIds = externalCountries.Select(c => c.Id).ToHashSet();
            var missingIds = externalIds.Except(internalIds).ToList();
            var extraIds = internalIds.Except(externalIds).ToList();
            var modifiedIds = internalIds.Intersect(externalIds).Where(id => {
                var internalCountry = internalCountries.First(c => c.Id == id);
                var externalCountry = externalCountries.First(c => c.Id == id);
                return internalCountry.Name != externalCountry.Name;
            }).ToList();

            // Apply the necessary changes to bring the two databases into sync
            foreach (var id in missingIds)
            {
                var externalCountry = externalCountries.First(c => c.Id == id);
                var internalCountry = new InternalCountry {Id = externalCountry.Id, Name = externalCountry.Name };
                _dbContext.InternalCountries.Add(internalCountry);
            }
            foreach (var id in extraIds)
            {
                var internalCountry = internalCountries.First(c => c.Id == id);
                _dbContext.InternalCountries.Remove(internalCountry);
            }
            foreach (var id in modifiedIds)
            {
                var externalCountry = externalCountries.First(c => c.Id == id);
                var internalCountry = internalCountries.First(c => c.Id == id);
                internalCountry.Name = externalCountry.Name;
            }
            await _dbContext.SaveChangesAsync();
        }

        private async Task SyncCitiesAsync()
        {
            await using var _externalDbContext = new ExternalDbContext();
            await using var _dbContext = new InternalDbContext();
            // Read the data from the internal database
            var internalCities = await _dbContext.InternalCities.ToListAsync();

            // Read the data from the external database
            var externalCities = await _externalDbContext.ExternalCities.ToListAsync();

            // Loop through the internal cities and update the corresponding external cities
            foreach (var internalCity in internalCities)
            {
                var externalCity = externalCities.FirstOrDefault(c => c.Id == internalCity.Id);

                if (externalCity == null)
                {
                    // If the city does not exist in the external database, create a new one
                    externalCity = new ExternalCity
                    {
                        Id = internalCity.Id,
                        Name = internalCity.Name,
                        CountryId = internalCity.CountryId
                    };
                    _externalDbContext.ExternalCities.Add(externalCity);
                }
                else
                {
                    // If the city already exists in the external database, update its properties
                    externalCity.Name = internalCity.Name;
                    externalCity.CountryId = internalCity.CountryId;
                    _externalDbContext.ExternalCities.Update(externalCity);
                }
            }

            // Loop through the external cities and remove the ones that do not exist in the internal database
            foreach (var externalCity in externalCities)
            {
                var internalCity = internalCities.FirstOrDefault(c => c.Id == externalCity.Id);

                if (internalCity == null)
                {
                    _externalDbContext.ExternalCities.Remove(externalCity);
                }
            }
            await _externalDbContext.SaveChangesAsync();
        }

        private async Task SyncOfficesAsync()
        {
            await using var _externalDbContext = new ExternalDbContext();
            await using var _dbContext = new InternalDbContext();
            // Read the data from the internal database
            var internalOffices = await _dbContext.InternalOffices.ToListAsync();

            // Read the data from the external database
            var externalOffices = await _externalDbContext.ExternalOffices.ToListAsync();

            // Loop through the internal offices and update the corresponding external offices
            foreach (var internalOffice in internalOffices)
            {
                var externalOffice = externalOffices.FirstOrDefault(c => c.Id == internalOffice.Id);

                if (externalOffice == null)
                {
                    // If the office does not exist in the external database, create a new one
                    externalOffice = new ExternalOffice
                    {
                        Name = internalOffice.Name,
                        CityId = internalOffice.CityId
                    };
                    _externalDbContext.ExternalOffices.Add(externalOffice);
                }
                else
                {
                    // If the office already exists in the external database, update its properties
                    externalOffice.Name = internalOffice.Name;
                    externalOffice.CityId = internalOffice.CityId;
                    _externalDbContext.ExternalOffices.Update(externalOffice);
                }
            }

            // Loop through the external offices and remove the ones that do not exist in the internal database
            foreach (var externalOffice in externalOffices)
            {
                var internalOffice = internalOffices.FirstOrDefault(c => c.Id == externalOffice.Id);

                if (internalOffice == null)
                {
                    _externalDbContext.ExternalOffices.Remove(externalOffice);
                }
            }
            await _externalDbContext.SaveChangesAsync();
        }
    }
}
