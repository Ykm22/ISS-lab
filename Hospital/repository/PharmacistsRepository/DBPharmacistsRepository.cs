using System.Collections.Generic;
using System.Data;
using System.Linq;
using model;
using NHibernate;

namespace repository.PharmacistsRepository
{
    public class DBPharmacistsRepository : IPharmacistsRepository<int, Pharmacist>
    {
        private ISessionFactory sessionFactory;
        public DBPharmacistsRepository(ISessionFactory sessionFactory)
        {
            this.sessionFactory = sessionFactory;
        }

        public Pharmacist Save(Pharmacist entity)
        {
            throw new System.NotImplementedException();
        }

        public Pharmacist Delete(int id)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<Pharmacist> GetAll()
        {
            throw new System.NotImplementedException();
        }

        public Pharmacist Find(int id)
        {
            throw new System.NotImplementedException();
        }

        public Pharmacist Update(int id, Pharmacist entity)
        {
            throw new System.NotImplementedException();
        }

        public Pharmacist FindByCredentials(string pharmacistName, string pharmacistPassword)
        {
            using (var session = sessionFactory.OpenSession())
            {
                var pharmacist = session.Query<Pharmacist>()
                    .FirstOrDefault(p => p.Name == pharmacistName && p.Password == pharmacistPassword);
                return pharmacist;
            }
            return null;
        }
    }
}