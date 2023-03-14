using System.ComponentModel.DataAnnotations.Schema;

namespace ExternalDb.Entities
{
    public class ExternalOffice
    {
        /// <summary>
        /// Gets or sets the id of external office table
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of external office table
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the cityId of external office table
        /// </summary>
        public int CityId { get; set; }
        public ExternalCity City { get; set; }
    }
}
