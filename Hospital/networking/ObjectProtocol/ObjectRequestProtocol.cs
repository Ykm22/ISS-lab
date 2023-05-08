using System;
using model;
using networking.DTO;

namespace networking.ObjectProtocol
{
    public interface Request {}
    [Serializable]
    public class LoginRequest : Request
    {
        private PharmacistDto pharmacistDto;

        public LoginRequest(PharmacistDto pharmacistDto)
        {
            this.pharmacistDto = pharmacistDto;
        }

        public virtual PharmacistDto PharmacistDto
        {
            get
            {
                return pharmacistDto;
            }
        }
    }
    [Serializable]
    public class AddMedicineRequest : Request
    {
        private MedicineDto medicineDto;

        public AddMedicineRequest(MedicineDto medicineDto)
        {
            this.medicineDto = medicineDto;
        }

        public virtual MedicineDto MedicineDto
        {
            get
            {
                return medicineDto;
            }
        }
    }
    [Serializable]
    public class UpdateMedicineRequest : Request
    {
        private MedicineDto medicineDto;

        public UpdateMedicineRequest(MedicineDto medicineDto)
        {
            this.medicineDto = medicineDto;
        }

        public virtual MedicineDto MedicineDto
        {
            get
            {
                return medicineDto;
            }
        }
    }
    [Serializable]
    public class DeleteMedicineRequest : Request
    {
        private int idToDelete;

        public DeleteMedicineRequest(int IdToDelete)
        {
            idToDelete = IdToDelete;
        }

        public virtual int IdToDelete
        {
            get
            {
                return idToDelete;
            }
        }
    }
    [Serializable]
    public class FilterMedicinesRequest : Request
    {
        private Purpose purpose;

        public FilterMedicinesRequest(Purpose purpose)
        {
            this.purpose = purpose;
        }

        public virtual Purpose Purpose
        {
            get
            {
                return purpose;
            }
        }
    }
    [Serializable]
    public class GetAllMedicinesRequest : Request
    {
        public GetAllMedicinesRequest()
        {
            
        }
    }
    [Serializable]
    public class FindPharmacistByCredentialsRequest : Request
    {
        private string pharmacistName;
        private string pharmacistPassword;

        public FindPharmacistByCredentialsRequest(string pharmacistName, string pharmacistPassword)
        {
            this.pharmacistName = pharmacistName;
            this.pharmacistPassword = pharmacistPassword;
        }

        public virtual string PharmacistName
        {
            get
            {
                return pharmacistName;
            }
        }
        public virtual string PharmacistPassword
        {
            get
            {
                return pharmacistPassword;
            }
        }
    }
    [Serializable]
    public class LogoutRequest : Request
    {
        private PharmacistDto pharmacistDto;

        public LogoutRequest(PharmacistDto pharmacistDto)
        {
            this.pharmacistDto = pharmacistDto;
        }

        public virtual PharmacistDto PharmacistDto
        {
            get
            {
                return pharmacistDto;
            }
        }
    }
}