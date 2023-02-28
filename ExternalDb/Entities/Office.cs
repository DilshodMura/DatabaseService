
namespace ExternalDb.Entities
{
    public class ExternalOffice
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CityId { get; set; }
        public ExternalCity City { get; set; }
    }
}
