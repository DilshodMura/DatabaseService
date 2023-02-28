
using System.ComponentModel.DataAnnotations.Schema;

namespace ExternalDb.Entities
{
    public class ExternalCountry
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<ExternalCity> Cities { get; set; } = new List<ExternalCity>();
        public List<ExternalOffice> Offices { get; set; } = new List<ExternalOffice>();
    }
}
