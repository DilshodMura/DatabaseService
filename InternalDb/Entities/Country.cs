
using System.ComponentModel.DataAnnotations.Schema;

namespace InternalDb.Entities
{
    public class InternalCountry
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public string Name { get; set; }
        public List<InternalCity> Cities { get; set; } = new List<InternalCity>();
        public List<InternalOffice> Offices { get; set; } = new List<InternalOffice>();
    }
}
