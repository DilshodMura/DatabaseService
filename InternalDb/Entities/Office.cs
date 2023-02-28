
namespace InternalDb.Entities
{
    public class InternalOffice
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CityId { get; set; }
        public InternalCity City { get; set; }
    }
}
