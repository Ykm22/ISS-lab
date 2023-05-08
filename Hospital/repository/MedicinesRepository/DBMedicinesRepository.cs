using System.Collections.Generic;
using System.Linq;
using model;
using NHibernate;

namespace repository.MedicinesRepository
{
    public class DBMedicinesRepository : IMedicinesRepository<int, Medicine>
    {
        private ISessionFactory _sessionFactory;
        public DBMedicinesRepository(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }

        public Medicine Save(Medicine entity)
        {
            using (var session = _sessionFactory.OpenSession())
            {
                session.Save(entity);
            }

            return entity;
        }

        public Medicine Delete(int id)
        {
            using (var session = _sessionFactory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    var medicine = session.Get<Medicine>(id);
                    if (medicine != null)
                    {
                        session.Delete(medicine);
                    }
                    transaction.Commit();
                    return medicine;
                }
                
            }
        }

        public IEnumerable<Medicine> GetAll()
        {
            using (var session = _sessionFactory.OpenSession())
            {
                ICriteria criteria = session.CreateCriteria(typeof(Medicine));
                IList<Medicine> medicines = criteria.List<Medicine>();
                return medicines;
            }
        }

        public IEnumerable<Medicine> FindByPurpose(Purpose purpose)
        {
            using (var session = _sessionFactory.OpenSession())
            {
                return session.Query<Medicine>()
                    .Where(m => m.Purpose == purpose)
                    .ToList();
            }
        }

        public Medicine Find(int id)
        {
            throw new System.NotImplementedException();
        }

        public Medicine Update(int id, Medicine entity)
        {
            using (var session = _sessionFactory.OpenSession())
            {
                var medicine = session.Get<Medicine>(id);
                if (medicine != null)
                {
                    medicine.Name = entity.Name;
                    medicine.Purpose = entity.Purpose;
                    medicine.AvailableQuantity = entity.AvailableQuantity;
                    
                    session.Update(medicine);
                    session.Flush();
                }

                return medicine;
            }
        }
    }
}