using System;
using model;

namespace services
{
    public interface IServices : IMedicineServices, IPharmacistServices
    {
        Pharmacist Login(Pharmacist pharmacist, IObserver client);
        void Logout(Pharmacist pharmacist, IObserver client);
    }
}