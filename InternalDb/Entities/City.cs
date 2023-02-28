
namespace InternalDb.Entities
{
    public class InternalCity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CountryId { get; set; }
        public InternalCountry Country { get; set; }
        public List<InternalOffice> Offices { get; set; } = new List<InternalOffice>();
    }
}
