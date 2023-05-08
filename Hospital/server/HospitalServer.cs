using System;
using System.Configuration;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using model;
using networking;
using NHibernate.Dialect;
using NHibernate.Driver;
using repository.MedicinesRepository;
using repository.PharmacistsRepository;
using services;
using Configuration = NHibernate.Cfg.Configuration;

namespace server
{
    public class HospitalServer
    {
        static void Main(string[] args)
        {
            // using (var session = NHibernateHelper.GetSession())
            // {
            //     session.Save(new Medicine(Purpose.Headache, "Aspirin", 300));
            //     session.Save(new Medicine(Purpose.Headache, "Ibuprofen", 200));
            //     session.Save(new Medicine(Purpose.Headache, "Naproxen", 150));
            //     session.Save(new Medicine(Purpose.Stomachace, "Camylofin", 20));
            //     session.Save(new Medicine(Purpose.Stomachace, "Dexlansoprazole", 130));
            //     session.Save(new Medicine(Purpose.Stomachace, "Drotaverine", 30));
            //     session.Save(new Medicine(Purpose.SoreThroat, "Actamin", 20));
            //     session.Save(new Medicine(Purpose.SoreThroat, "Tylenol", 40));
            //     
            // }
            IMedicinesRepository<int, Medicine> medicinesRepository = new DBMedicinesRepository(NHibernateHelper.SessionFactory);
            IPharmacistsRepository<int, Pharmacist> pharmacistsRepository = new DBPharmacistsRepository(NHibernateHelper.SessionFactory);
            IServices services = new Services(medicinesRepository, pharmacistsRepository);
            SerialServer server = new SerialServer("127.0.0.1", 55556, services);
            server.Start();
            Console.WriteLine("Server started...");
        }
    }
    public class SerialServer : ConcurrentServer
    {
        private IServices server;
        private ClientWorker worker;

        public SerialServer(string host, int port, IServices server) : base(host, port)
        {
            this.server = server;
            Console.WriteLine("Serial server ...");
        }

        protected override Thread createWorker(TcpClient client)
        {
            worker = new ClientWorker(server, client);
            return new Thread(new ThreadStart(worker.run));
        }
    }
}