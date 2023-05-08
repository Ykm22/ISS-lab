using FluentNHibernate.Mapping;

namespace model
{
    public class MedicineMap : ClassMap<Medicine>
    {
        public MedicineMap()
        {
            Table("Medicines");
            Id(m => m.Id);
            Map(m => m.Name);
            Map(m => m.Purpose);
            Map(m => m.AvailableQuantity);
        }
    }
}