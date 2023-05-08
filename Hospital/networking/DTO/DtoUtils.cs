using System.Collections.Generic;
using model;

namespace networking.DTO
{
    public class DtoUtils
    {
        public static Medicine GetFromDto(MedicineDto medicineDto)
        {
            Medicine medicine = new Medicine(
                medicineDto.Purpose,
                medicineDto.Name,
                medicineDto.AvailableQuantity);
            medicine.SetId(medicineDto.Id);
            return medicine;
        }

        public static MedicineDto GetDto(Medicine medicine)
        {
            return new MedicineDto(
                medicine.GetId(),
                medicine.Purpose,
                medicine.Name,
                medicine.AvailableQuantity);
        }

        public static Pharmacist GetFromDto(PharmacistDto pharmacistDto)
        {
            Pharmacist pharmacist = new Pharmacist(
                pharmacistDto.Name,
                pharmacistDto.Password);
            pharmacist.SetId(pharmacistDto.Id);
            return pharmacist;
        }

        public static PharmacistDto GetDto(Pharmacist pharmacist)
        {
            return new PharmacistDto(
                pharmacist.GetId(),
                pharmacist.Name,
                pharmacist.Password);
        }
        public static IList<Medicine> GetFromDto(IList<MedicineDto> medicinesDto){
            IList<Medicine> medicines = new List<Medicine>();
            foreach(MedicineDto medicineDto in medicinesDto){
                Medicine medicine = GetFromDto(medicineDto);
                medicines.Add(medicine);
            }
            return medicines;
        }
        public static IList<MedicineDto> GetDto(IEnumerable<Medicine> medicines){
            IList<MedicineDto> medicinesDto = new List<MedicineDto>();
            foreach(Medicine medicine in medicines){
                MedicineDto medicineDto = GetDto(medicine);
                medicinesDto.Add(medicineDto);
            }
            return medicinesDto;
        }
    }
}