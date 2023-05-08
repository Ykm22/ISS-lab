using System;
using System.Collections.Generic;
using model;
using networking.DTO;

namespace networking.ObjectProtocol
{
    public interface Response{ }
    public interface UpdateResponse : Response
    {
    }

    [Serializable]
    public class UpdateAddedMedicineResponse : UpdateResponse
    {
        private MedicineDto medicineDto;

        public UpdateAddedMedicineResponse(MedicineDto medicineDto)
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
    public class UpdateUpdatedMedicineResponse : UpdateResponse
    {
        private MedicineDto medicineDto;

        public UpdateUpdatedMedicineResponse(MedicineDto medicineDto)
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
    public class UpdateDeletedMedicineResponse : UpdateResponse
    {
        private MedicineDto medicineDto;

        public UpdateDeletedMedicineResponse(MedicineDto medicineDto)
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
    public class GetAllMedicinesResponse : Response
    {
        private IList<MedicineDto> medicinesDto;

        public GetAllMedicinesResponse(IList<MedicineDto> medicinesDto)
        {
            this.medicinesDto = medicinesDto;
        }

        public virtual IList<MedicineDto> MedicinesDto
        {
            get
            {
                return medicinesDto;
            }
        }
    }
    [Serializable]
    public class FilterMedicinesResponse : Response
    {
        private IList<MedicineDto> medicinesDto;

        public FilterMedicinesResponse(IList<MedicineDto> medicinesDto)
        {
            this.medicinesDto = medicinesDto;
        }

        public virtual IList<MedicineDto> MedicinesDto
        {
            get
            {
                return medicinesDto;
            }
        }
    }
    [Serializable]
    public class OkResponse : Response
    { }
    [Serializable]
    public class ErrorResponse : Response
    {
        private string message;

        public ErrorResponse(string Message)
        {
            message = Message;
        }

        public virtual string Message
        {
            get
            {
                return message;
            }
        }
    }
    [Serializable]
    public class FindPharmacistByCredentialsResponse : Response
    {
        private PharmacistDto pharmacistDto;

        public FindPharmacistByCredentialsResponse(PharmacistDto pharmacistDto)
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