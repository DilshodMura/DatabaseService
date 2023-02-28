
using System.ComponentModel.DataAnnotations.Schema;

namespace ExternalDb.Entities
{
    public class ExternalCity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public string Name { get; set; }
        public int CountryId { get; set; }
        public ExternalCountry Country { get; set; }
        public List<ExternalOffice> Offices { get; set; } = new List<ExternalOffice>();
    }
}
