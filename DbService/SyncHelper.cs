using ExternalDb;
using ExternalDb.Entities;
using InternalDb;
using InternalDb.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace DbService
{
    public class SyncHelper:BackgroundService
    {
        private readonly ILogger<SyncHelper> _logger;

        public SyncHelper(ILogger<SyncHelper> logger)
        {
            _logger = logger;
        }
        /// <summary>
        /// Executing methods to sync the data
        /// </summary>
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
        /// <summary>
        /// Synchronize country tables
        /// </summary>
        private async Task SyncCountriesAsync()
        {
            await using var _externalDbContext = new ExternalDbContext();
            await using var _dbContext = new InternalDbContext();
            // Read the data from the internal database
            var internalCountries = await _dbContext.InternalCountries.ToListAsync();

            // Read the data from the external database
            var externalCountries = await _externalDbContext.ExternalCountries.ToListAsync();

            // Identify the differences between the two sets of data
            var internalIds = internalCountries.Select(c => c.Id).ToList();
            var externalIds = externalCountries.Select(c => c.Id).ToList();
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
                var internalCountry = new InternalCountry { Id = externalCountry.Id, Name = externalCountry.Name };
                _dbContext.InternalCountries.Add(internalCountry);
            }
            var createTableSql = @"
                IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'DeletedCountries')
                BEGIN
                    CREATE TABLE DeletedCountries (
                        Id INT,
                        Name NVARCHAR(100) NOT NULL
                    )
                END
                IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'DeletedCities')
                BEGIN
                    CREATE TABLE DeletedCities (
                    Id INT,
                    Name NVARCHAR(100) NOT NULL,
                    CountryId INT NOT NULL
                )
                END
                IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'DeletedOffices')
                BEGIN
                    CREATE TABLE DeletedOffices (
                    Id INT,
                    Name NVARCHAR(100) NOT NULL,
                    CityId INT NOT NULL
                )
                END
            ";

            _dbContext.Database.ExecuteSqlRaw(createTableSql);
            _externalDbContext.Database.ExecuteSqlRaw(createTableSql);

            var insertDeletedCountrySql = @"
                INSERT INTO DeletedCountries (Id, Name)
                VALUES (@id, @name)
            ";
            var insertDeletedCitySql = @"
                INSERT INTO DeletedCities (Id, Name, CountryId)
                VALUES (@id, @name, @countryId)
            ";
            var insertDeletedOfficeSql = @"
                INSERT INTO DeletedOffices (Id, Name, CityId)
                VALUES (@id, @name, @cityId)
            ";

            var parameters = new[]
            {
                new SqlParameter("@id", SqlDbType.Int),
                new SqlParameter("@name", SqlDbType.NVarChar, 100),
                new SqlParameter("@countryId", SqlDbType.Int),
                new SqlParameter("@cityId", SqlDbType.Int)
            };

            foreach (var id in extraIds)
            {
                var internalCountry = internalCountries.First(c => c.Id == id);

                // Set the parameter values for the SQL command
                parameters[0].Value = internalCountry.Id;
                parameters[1].Value = internalCountry.Name;
                parameters[2].Value = 0;
                parameters[3].Value = 0;
                // Execute the SQL command using ExecuteSqlRaw
                _dbContext.Database.ExecuteSqlRaw(insertDeletedCountrySql, parameters);
                _externalDbContext.Database.ExecuteSqlRaw(insertDeletedCountrySql, parameters);

                // Remove the deleted country from the internal database
                _dbContext.InternalCountries.Remove(internalCountry);

                // Remove linked cities and offices from the internal database
                var internalCities = await _dbContext.InternalCities
                                                        .Include(c => c.Offices)
                                                        .Where(c => c.CountryId == internalCountry.Id)
                                                        .ToListAsync();

                foreach (var internalCity in internalCities)
                {
                    parameters[0].Value = internalCity.Id;
                    parameters[1].Value = internalCity.Name;
                    parameters[2].Value = internalCity.CountryId;

                    // Execute the SQL command using ExecuteSqlRaw
                    _dbContext.Database.ExecuteSqlRaw(insertDeletedCitySql, parameters);
                    _externalDbContext.Database.ExecuteSqlRaw(insertDeletedCitySql, parameters);

                    // Remove linked offices from the internal database
                    var internalOffices = await _dbContext.InternalOffices.Where(o => o.CityId == internalCity.Id).ToListAsync();
                    foreach (var internalOffice in internalOffices)
                    {
                        parameters[0].Value = internalOffice.Id;
                        parameters[1].Value = internalOffice.Name;
                        parameters[3].Value = internalOffice.CityId;

                        // Execute the SQL command using ExecuteSqlRaw
                        _dbContext.Database.ExecuteSqlRaw(insertDeletedOfficeSql, parameters);
                        _externalDbContext.Database.ExecuteSqlRaw(insertDeletedOfficeSql, parameters);

                        // Remove the deleted office from the internal database
                        _dbContext.InternalOffices.Remove(internalOffice);
                    }

                    // Remove the deleted city from the internal database
                    _dbContext.InternalCities.Remove(internalCity);
                }
            }

            foreach (var id in modifiedIds)
            {
                var externalCountry = externalCountries.First(c => c.Id == id);
                var internalCountry = internalCountries.First(c => c.Id == id);
                internalCountry.Name = externalCountry.Name;
            }
            await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Synchronize city tables
        /// </summary>
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
                    await _externalDbContext.ExternalCities.AddAsync(externalCity);
                }
                else
                {
                    // If the city already exists in the external database, update its properties
                    externalCity.Name = internalCity.Name;
                    externalCity.CountryId = internalCity.CountryId;
                    _externalDbContext.ExternalCities.UpdateRange(externalCity);
                }
            }
            await _externalDbContext.SaveChangesAsync();

            // Check if DeletedCities table exists and create it if it doesn't
            await _externalDbContext.Database.ExecuteSqlRawAsync(@"
                    IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'DeletedCities')
                BEGIN
                    CREATE TABLE DeletedCities (
                    Id INT,
                    Name NVARCHAR(100) NOT NULL,
                    CountryId INT NOT NULL
                )
                END
                ");

            // Check if DeletedOffices table exists and create it if it doesn't
                await _externalDbContext.Database.ExecuteSqlRawAsync(@"
                    IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'DeletedOffices')
                BEGIN
                    CREATE TABLE DeletedOffices (
                    Id INT,
                    Name NVARCHAR(100) NOT NULL,
                    CityId INT NOT NULL
                )
                END
                ");

            // Loop through internal cities and delete corresponding external cities and offices
            foreach (var externalCity in externalCities)
            {
                var internalCity = internalCities.FirstOrDefault(c => c.Id == externalCity.Id);

                if (internalCity == null)
                {
                    // City does not exist in internal database, delete it and its linked offices
                    var externalOffices = await _externalDbContext.ExternalOffices.Where(o => o.CityId == externalCity.Id).ToListAsync();
                    foreach (var externalOffice in externalOffices)
                    {
                        // Add the office to DeletedOffices table
                        await _externalDbContext.Database.ExecuteSqlRawAsync(@"
                        INSERT INTO DeletedOffices (Id, Name, CityId)
                        VALUES (@id, @name, @cityId)
                    ",
                        new SqlParameter("@id", externalOffice.Id),
                        new SqlParameter("@name", externalOffice.Name),
                        new SqlParameter("@cityId", externalOffice.CityId));
                        // Remove the office from the external database
                        _externalDbContext.ExternalOffices.Remove(externalOffice);
                    }
                    // Add the city to DeletedCities table
                    await _externalDbContext.Database.ExecuteSqlRawAsync(@"
                    INSERT INTO DeletedCities (Id, Name, CountryId)
                    VALUES (@id, @name, @countryId)
                ",
                    new SqlParameter("@id", externalCity.Id),
                    new SqlParameter("@name", externalCity.Name),
                    new SqlParameter("@countryId", externalCity.CountryId));
                    // Remove the city from the external database
                    _externalDbContext.ExternalCities.Remove(externalCity);
                }
            }
            // Save changes to external database
            await _externalDbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Synchronize office tables
        /// </summary>
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
                        Id = internalOffice.Id,
                        Name = internalOffice.Name,
                        CityId = internalOffice.CityId,
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
                    // If the DeletedOffices table does not exist, create it
                    if (!_externalDbContext.Database.GetPendingMigrations().Any(m => m == "CreateDeletedOfficesTable"))
                    {
                        await _externalDbContext.Database.ExecuteSqlRawAsync(@"
                             IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'DeletedOffices')
                            BEGIN
                                CREATE TABLE DeletedOffices (
                                Id INT,
                                Name NVARCHAR(100) NOT NULL,
                                CityId INT NOT NULL
                            )
                            END
                        ");
                    }

                    // Add the office to DeletedOffices table
                    await _externalDbContext.Database.ExecuteSqlRawAsync(@"
                        INSERT INTO DeletedOffices (Id, Name, CityId)
                        VALUES (@id, @name, @cityId)
                    ",
                    new SqlParameter("@id", externalOffice.Id),
                    new SqlParameter("@name", externalOffice.Name),
                    new SqlParameter("@cityId", externalOffice.CityId));

                    // Remove the office from the external database
                    _externalDbContext.ExternalOffices.Remove(externalOffice);
                }
            }
            await _externalDbContext.SaveChangesAsync();
        }
    }
}
