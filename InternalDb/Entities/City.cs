
namespace InternalDb.Entities
{
    public class InternalCity
    {
        /// <summary>
        /// Gets or sets the id for internal city table
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name for internal city table
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the countryId for internal city table
        /// </summary>
        public int CountryId { get; set; }
        public InternalCountry Country { get; set; }

        /// <summary>
        /// Gets or sets the list of offices for internal city table
        /// </summary>
        public List<InternalOffice> Offices { get; set; } = new List<InternalOffice>();
    }
}
