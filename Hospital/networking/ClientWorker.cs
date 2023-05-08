using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using model;
using networking.DTO;
using networking.ObjectProtocol;
using services;

namespace networking
{
    public class ClientWorker : IObserver
    {
        private IServices server;
        private TcpClient connection;

        private NetworkStream stream;
        private IFormatter formatter;
        private volatile bool connected;
        
        public ClientWorker(IServices server, TcpClient connection)
        {
            this.server = server;
            this.connection = connection;
            try
            {
                stream = connection.GetStream();
                formatter = new BinaryFormatter();
                connected = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
        }
        public virtual void run()
        {
            while (connected)
            {
                try
                {
                    object request = formatter.Deserialize(stream);
                    object response = handleRequest((Request)request);
                    if (response != null)
                    {
                        sendResponse((Response)response);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.StackTrace);
                }

                // try
                // {
                //     Thread.Sleep(1000);
                // }
                // catch (Exception e)
                // {
                //     Console.WriteLine(e.StackTrace);
                // }
            }

            try
            {
                stream.Close();
                connection.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("error " + e);
            }
        }
        private void sendResponse(Response Response)
        {
            Console.WriteLine("sending response " + Response);
            lock (stream)
            {
                formatter.Serialize(stream, Response);
                stream.Flush();
            }
        }
        private Response handleRequest(Request request)
        {
            Response response = null;
            if (request is FindPharmacistByCredentialsRequest)
            {
                FindPharmacistByCredentialsRequest req = (FindPharmacistByCredentialsRequest)request;
                return handleFIND_PHARMACIST_BY_CREDENTIALS(req);
            }
            if (request is LoginRequest)
            {
                LoginRequest req = (LoginRequest)request;
                return handleLOGIN(req);
            }
            //
            // if (request is LogoutRequest)
            // {
            //     LogoutRequest req = (LogoutRequest)request;
            //     return handleLOGOUT(req);
            // }
            //
            if (request is GetAllMedicinesRequest)
            {
                GetAllMedicinesRequest req = (GetAllMedicinesRequest)request;
                return handleGET_ALL_MEDICINES(req);
            }
            if (request is FilterMedicinesRequest)
            {
                FilterMedicinesRequest req = (FilterMedicinesRequest)request;
                return handleFILTER_MEDICINES(req);
            }
            if (request is DeleteMedicineRequest)
            {
                DeleteMedicineRequest req = (DeleteMedicineRequest)request;
                return handleDELETE_MEDICINE(req);
            }
            //
            // if (request is FindArtistRequest)
            // {
            //     FindArtistRequest req = (FindArtistRequest)request;
            //     return handleFIND_ARTIST(req);
            // }
            //
            // if (request is FindSpectaclesByDayRequest)
            // {
            //     FindSpectaclesByDayRequest req = (FindSpectaclesByDayRequest)request;
            //     return handleFIND_SPECTACLES_BY_DAY(req);
            // }
            //
            // if (request is FindSpectacleRequest)
            // {
            //     FindSpectacleRequest req = (FindSpectacleRequest)request;
            //     return handleFIND_SPECTACLE(req);
            // }
            //
            if (request is AddMedicineRequest)
            {
                AddMedicineRequest req = (AddMedicineRequest)request;
                return handleADD_MEDICINE(req);
            }
            
            if (request is UpdateMedicineRequest)
            {
                UpdateMedicineRequest req = (UpdateMedicineRequest)request;
                return handleUPDATE_MEDICINE(req);
            }

            return response;
        }

        private Response handleFILTER_MEDICINES(FilterMedicinesRequest req)
        {
            Console.WriteLine("Filter medicines request...");
            try
            {
                IEnumerable<Medicine> medicines;
                lock (server)
                {
                    medicines = server.FilterMedicines(req.Purpose);
                }

                IList<MedicineDto> medicinesDto = DtoUtils.GetDto(medicines);
                return new FilterMedicinesResponse(medicinesDto);
            }
            catch (HospitalException e)
            {
                return new ErrorResponse(e.Message);
            }
        }

        private Response handleDELETE_MEDICINE(DeleteMedicineRequest req)
        {
            int IdToDelete = req.IdToDelete;
            try
            {
                lock (server)
                {
                    server.DeleteMedicine(IdToDelete);
                }
                return new OkResponse();
            }
            catch (HospitalException e)
            {
                return new ErrorResponse(e.Message);
            }
        }

        private Response handleUPDATE_MEDICINE(UpdateMedicineRequest req)
        {
            Medicine medicine = DtoUtils.GetFromDto(req.MedicineDto);
            try
            {
                lock (server)
                {
                    server.UpdateMedicine(medicine);
                }

                return new OkResponse();
            }
            catch (HospitalException e)
            {
                return new ErrorResponse(e.Message);
            }
        }

        private Response handleADD_MEDICINE(AddMedicineRequest req)
        {
            Console.WriteLine("Add medicine request...");
            Medicine medicine = DtoUtils.GetFromDto(req.MedicineDto);
            try
            {
                lock (server)
                {
                    server.AddMedicine(medicine);
                }

                return new OkResponse();
            }
            catch (HospitalException e)
            {
                return new ErrorResponse(e.Message);
            }
        }

        private Response handleGET_ALL_MEDICINES(GetAllMedicinesRequest req)
        {
            Console.WriteLine("Get all medicines request...");
            try
            {
                IEnumerable<Medicine> medicines;
                lock (server)
                {
                    medicines = server.GetAllMedicines();
                }

                IList<MedicineDto> medicinesDto = DtoUtils.GetDto(medicines);
                return new GetAllMedicinesResponse(medicinesDto);
            }
            catch (HospitalException e)
            {
                return new ErrorResponse(e.Message);
            }
        }

        private Response handleFIND_PHARMACIST_BY_CREDENTIALS(FindPharmacistByCredentialsRequest req)
        {
            Console.WriteLine("Find pharmacist by credentials request...");
            string pharmacistName = req.PharmacistName;
            string pharmacistPassword = req.PharmacistPassword;
            try
            {
                Pharmacist pharmacist = null;
                lock (server)
                {
                    pharmacist = server.FindPharmacistByCredentials(pharmacistName, pharmacistPassword);
                }

                return new FindPharmacistByCredentialsResponse(DtoUtils.GetDto(pharmacist));
            }
            catch (HospitalException ex)
            {
                return new ErrorResponse(ex.Message);
            }
        }

        private Response handleLOGIN(LoginRequest req)
        {
            Console.WriteLine("Login request...");
            Pharmacist ticketAgent = DtoUtils.GetFromDto(req.PharmacistDto);
            try
            {
                lock (server)
                {
                    server.Login(ticketAgent, this);
                }
                return new OkResponse();
            }
            catch (HospitalException e)
            {
                connected = false;
                return new ErrorResponse(e.Message);
            }
        }

        public void Update_AddedMedicine(Medicine medicine)
        {
            try
            {
                MedicineDto medicineDto = DtoUtils.GetDto(medicine);
                sendResponse(new UpdateAddedMedicineResponse(medicineDto));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
        }

        public void Update_UpdatedMedicine(Medicine medicine)
        {
            try
            {
                MedicineDto medicineDto = DtoUtils.GetDto(medicine);
                sendResponse(new UpdateUpdatedMedicineResponse(medicineDto));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
        }
        public void Update_DeletedMedicine(Medicine medicine)
        {
            try
            {
                MedicineDto medicineDto = DtoUtils.GetDto(medicine);
                sendResponse(new UpdateDeletedMedicineResponse(medicineDto));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
        }
    }
}