using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using model;
using services;

namespace client
{
    public class PharmacyController : IObserver
    {
        public event EventHandler<UserEventArgs> updateEvent;
        private readonly IServices server;
        private Pharmacist loggedPharmacist;

        public PharmacyController(IServices server)
        {
            this.server = server;
            loggedPharmacist = null;
        }
        public Pharmacist Login(Pharmacist pharmacist)
        {
            loggedPharmacist = server.Login(pharmacist, this);
            Console.WriteLine("Current user {0}", loggedPharmacist);
            return loggedPharmacist;
        }

        public void Update_AddedMedicine(Medicine addedMedicine)
        {
            UserEventArgs userArgs = new UserEventArgs(UserEvent.Update_AddedMedicine, addedMedicine);
            OnUserEvent(userArgs);
        }
        public void Update_UpdatedMedicine(Medicine addedMedicine)
        {
            UserEventArgs userArgs = new UserEventArgs(UserEvent.Update_UpdatedMedicine, addedMedicine);
            OnUserEvent(userArgs);
        }
        public void Update_DeletedMedicine(Medicine addedMedicine)
        {
            UserEventArgs userArgs = new UserEventArgs(UserEvent.Update_DeletedMedicine, addedMedicine);
            OnUserEvent(userArgs);
        }
        protected virtual void OnUserEvent(UserEventArgs e)
        {
            if (updateEvent == null) return;
            updateEvent(this, e);
        }
        public IList<Medicine> GetAllMedicines()
        {
            return server.GetAllMedicines().ToList();
        }

        public void AddMedicine(Medicine medicine)
        {
            server.AddMedicine(medicine);
        }

        public void UpdateMedicine(Medicine medicine)
        {
            server.UpdateMedicine(medicine);
        }

        public void DeleteMedicine(int idToDelete)
        {
            server.DeleteMedicine(idToDelete);
        }

        public IList<Medicine> FilterMedicines(Purpose purpose)
        {
            return server.FilterMedicines(purpose).ToList();
        }
    }
}