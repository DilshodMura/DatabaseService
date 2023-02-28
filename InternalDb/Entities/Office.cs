
namespace InternalDb.Entities
{
    public class InternalOffice
    {
        /// <summary>
        /// Gets or sets the id for internal office table
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name for internal office table
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the cityId for internal office table
        /// </summary>
        public int CityId { get; set; }
        public InternalCity City { get; set; }
    }
}
