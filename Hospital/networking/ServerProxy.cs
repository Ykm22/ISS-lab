using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.ServiceModel.Channels;
using System.Threading;
using System.Windows.Forms;
using model;
using networking.DTO;
using networking.ObjectProtocol;
using services;
using Message = System.Windows.Forms.Message;

namespace networking
{
    public class ServerProxy : IServices
    {
        private string host;
        private int port;

        private IObserver client;
        private NetworkStream stream;

        private IFormatter formatter;
        private TcpClient connection;

        private Queue<Response> responses;
        private volatile bool finished;
        private EventWaitHandle _waitHandle;
        
        public ServerProxy(string Host, int Port)
        {
            host = Host;
            port = Port;
            this.responses = new Queue<Response>();
        }
        private void sendRequest(Request request)
        {
            try
            {
                formatter.Serialize(stream, request);
                stream.Flush();
            }
            catch (Exception e)
            {
                throw new HospitalException("Error sending object" + e);
            }
        }
        private Response readResponse()
        {
            Response response = null;
            try
            {
                _waitHandle.WaitOne();
                lock (responses)
                {
                    response = responses.Dequeue();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }

            return response;
        }
        private void initializeConnection()
        {
            try
            {
                connection = new TcpClient(host, port);
                stream = connection.GetStream();
                formatter = new BinaryFormatter();
                finished = false;
                _waitHandle = new AutoResetEvent(false);
                startReader();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
        }
        private void closeConnection()
        {
            finished = true;
            try
            {
                stream.Close();
                connection.Close();
                _waitHandle.Close();
                client = null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
        }
        private void startReader()
        {
            Thread tw = new Thread(run);
            tw.Start();
        }
        public virtual void run()
        {
            while (!finished)
            {
                try
                {
                    object response = formatter.Deserialize(stream);
                    if (response is UpdateResponse)
                    {
                        handleUpdate((UpdateResponse)response);
                    }
                    else
                    {
                        lock (responses)
                        {
                            responses.Enqueue((Response)response);
                        }

                        _waitHandle.Set();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Reading error" + e);
                }
            }
        }
        
        // todo
        private void handleUpdate(UpdateResponse Response)
        {
            try
            {
                if (Response is UpdateAddedMedicineResponse)
                {
                    UpdateAddedMedicineResponse resp = (UpdateAddedMedicineResponse)Response;
                    Medicine addedMedicine = DtoUtils.GetFromDto(resp.MedicineDto);
                    client.Update_AddedMedicine(addedMedicine);
                }

                if (Response is UpdateUpdatedMedicineResponse)
                {
                    UpdateUpdatedMedicineResponse resp = (UpdateUpdatedMedicineResponse)Response;
                    Medicine updatedMedicine = DtoUtils.GetFromDto(resp.MedicineDto);
                    client.Update_UpdatedMedicine(updatedMedicine);
                }
                if (Response is UpdateDeletedMedicineResponse)
                {
                    UpdateDeletedMedicineResponse resp = (UpdateDeletedMedicineResponse)Response;
                    Medicine updatedMedicine = DtoUtils.GetFromDto(resp.MedicineDto);
                    client.Update_DeletedMedicine(updatedMedicine);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
        }

        public IEnumerable<Medicine> GetAllMedicines()
        {
            sendRequest(new GetAllMedicinesRequest());
            Response response = readResponse();
            if (response is ErrorResponse)
            {
                ErrorResponse err = (ErrorResponse)response;
                throw new HospitalException(err.Message);
            }

            GetAllMedicinesResponse resp = (GetAllMedicinesResponse)response;
            return DtoUtils.GetFromDto(resp.MedicinesDto);
        }

        public void AddMedicine(Medicine medicine)
        {
            MedicineDto medicineDto = DtoUtils.GetDto(medicine);
            sendRequest(new AddMedicineRequest(medicineDto));
            Response response = readResponse();
            if (response is ErrorResponse)
            {
                ErrorResponse err = (ErrorResponse)response;
                throw new HospitalException(err.Message);
            }
        }

        public void UpdateMedicine(Medicine medicine)
        {
            MedicineDto medicineDto = DtoUtils.GetDto(medicine);
            sendRequest(new UpdateMedicineRequest(medicineDto));
            Response response = readResponse();
            if (response is ErrorResponse)
            {
                ErrorResponse err = (ErrorResponse)response;
                throw new HospitalException(err.Message);
            }
        }

        public void DeleteMedicine(int id)
        {
            sendRequest(new DeleteMedicineRequest(id));
            Response response = readResponse();
            if (response is ErrorResponse)
            {
                ErrorResponse err = (ErrorResponse)response;
                throw new HospitalException(err.Message);
            }
        }

        public IEnumerable<Medicine> FilterMedicines(Purpose purpose)
        {
            sendRequest(new FilterMedicinesRequest(purpose));
            Response response = readResponse();
            if (response is ErrorResponse)
            {
                ErrorResponse err = (ErrorResponse)response;
                throw new HospitalException(err.Message);
            }

            FilterMedicinesResponse resp = (FilterMedicinesResponse)response;
            IList<Medicine> medicines =  DtoUtils.GetFromDto(resp.MedicinesDto);
            return medicines;
        }

        public Pharmacist Login(Pharmacist pharmacist, IObserver client)
        {
            initializeConnection();
            Pharmacist foundPharmacist = FindPharmacistByCredentials(pharmacist.Name, pharmacist.Password);
            PharmacistDto pharmacistDto = DtoUtils.GetDto(foundPharmacist);
            sendRequest(new LoginRequest(pharmacistDto));
            
            Response response = readResponse();
            if (response is OkResponse)
            {
                this.client = client;
                return foundPharmacist;
            }

            if (response is ErrorResponse)
            {
                ErrorResponse err = (ErrorResponse)response;
                closeConnection();
                throw new HospitalException(err.Message);
            }

            return null;
        }

        public Pharmacist FindPharmacistByCredentials(string pharmacistName, string pharmacistPassword)
        {
            sendRequest(new FindPharmacistByCredentialsRequest(pharmacistName, pharmacistPassword));
            Response response = readResponse();
            if (response is ErrorResponse)
            {
                ErrorResponse err = (ErrorResponse)response;
                closeConnection();
                throw new HospitalException(err.Message);
            }

            FindPharmacistByCredentialsResponse resp = (FindPharmacistByCredentialsResponse)response;
            PharmacistDto pharmacistDto = resp.PharmacistDto;
            Pharmacist pharmacist = DtoUtils.GetFromDto(pharmacistDto);
            return pharmacist;
        }

        public void Logout(Pharmacist pharmacist, IObserver client)
        {
            PharmacistDto pharmacistDto = DtoUtils.GetDto(pharmacist);
            sendRequest(new LogoutRequest(pharmacistDto));
            Response response = readResponse();
            closeConnection();
            if (response is ErrorResponse)
            {
                ErrorResponse err = (ErrorResponse)response;
                throw new HospitalException(err.Message);
            }
        }
    }
}