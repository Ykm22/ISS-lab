using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure.Design;
using System.Threading.Tasks;
using model;
using repository.MedicinesRepository;
using repository.PharmacistsRepository;
using services;

namespace server
{
    public class Services : IServices
    {
        private IMedicinesRepository<int, Medicine> _medicinesRepository;
        private IPharmacistsRepository<int, Pharmacist> _pharmacistsRepository;
        private readonly IDictionary<int, IObserver> LoggedClients;

        public Services(IMedicinesRepository<int, Medicine> medicinesRepository, IPharmacistsRepository<int, Pharmacist> pharmacistsRepository)
        {
            _medicinesRepository = medicinesRepository;
            _pharmacistsRepository = pharmacistsRepository;
            LoggedClients = new Dictionary<int, IObserver>();
        }

        public IEnumerable<Medicine> GetAllMedicines()
        {
            return _medicinesRepository.GetAll();
        }

        public void AddMedicine(Medicine medicine)
        {
            Medicine afterSave_Medicine = _medicinesRepository.Save(medicine);
            NotifyClients(UpdateType.AddMedicine, afterSave_Medicine);
        }

        private void NotifyClients(UpdateType updateType, Medicine medicine)
        {
            foreach (IObserver client in LoggedClients.Values)
            {
                if (updateType == UpdateType.AddMedicine)
                {
                    Task.Run(() => client.Update_AddedMedicine(medicine));
                }

                else if (updateType == UpdateType.UpdateMedicine)
                {
                    Task.Run(() => client.Update_UpdatedMedicine(medicine));
                }
                else if (updateType == UpdateType.DeleteMedicine)
                {
                    Task.Run(() => client.Update_DeletedMedicine(medicine));
                }
            }
        }

        public void UpdateMedicine(Medicine medicine)
        {
            _medicinesRepository.Update(medicine.Id, medicine);
            NotifyClients(UpdateType.UpdateMedicine, medicine);
        }

        public void DeleteMedicine(int id)
        {
            _medicinesRepository.Delete(id);
            Medicine temporary = new Medicine(Purpose.Headache, "", 0);
            temporary.SetId(id);
            NotifyClients(UpdateType.DeleteMedicine, temporary);
        }

        public IEnumerable<Medicine> FilterMedicines(Purpose purpose)
        {
            return _medicinesRepository.FindByPurpose(purpose);
        }

        public Pharmacist Login(Pharmacist pharmacist, IObserver client)
        {
            try
            {
                Pharmacist foundPharmacist = _pharmacistsRepository.FindByCredentials(pharmacist.Name, pharmacist.Password);
                if (foundPharmacist != null)
                {
                    if (LoggedClients.ContainsKey(foundPharmacist.Id))
                        throw new HospitalException("Pharmacist already locked in.");
                    LoggedClients[foundPharmacist.Id] = client;
                    return foundPharmacist;
                }
            
                throw new HospitalException("Authentication failed!");
            }
            catch (Exception e)
            {
                throw new HospitalException(e.Message);
            }
        }

        public void Logout(Pharmacist pharmacist, IObserver client)
        {
            throw new System.NotImplementedException();
        }

        public Pharmacist FindPharmacistByCredentials(string pharmacistName, string pharmacistPassword)
        {
            Pharmacist pharmacist = _pharmacistsRepository.FindByCredentials(pharmacistName, pharmacistPassword);
            return pharmacist;
        }
    }
}