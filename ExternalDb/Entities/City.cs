
using System.ComponentModel.DataAnnotations.Schema;

namespace ExternalDb.Entities
{
    public class ExternalCity
    {
        /// <summary>
        /// Gets or sets the id for internal city table
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name for internal city table
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the countryId for internal city table
        /// </summary>
        public int CountryId { get; set; }
        public ExternalCountry Country { get; set; }

        /// <summary>
        /// Gets or sets the list of offices for internal city table
        /// </summary>
        public List<ExternalOffice> Offices { get; set; } = new List<ExternalOffice>();
    }
}
