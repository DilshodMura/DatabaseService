using System.ComponentModel.DataAnnotations.Schema;

namespace InternalDb.Entities
{
    public class InternalCountry
    {
        /// <summary>
        /// Gets or sets the id of internal country table
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the id of internal country name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the id of internal city table
        /// </summary>
        public List<InternalCity> Cities { get; set; } = new List<InternalCity>();

        /// <summary>
        /// Gets or sets the id of internal offices table
        /// </summary>
        public List<InternalOffice> Offices { get; set; } = new List<InternalOffice>();
    }
}
