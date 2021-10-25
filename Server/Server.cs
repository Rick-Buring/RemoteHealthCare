using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using CommunicationObjects.DataObjects;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System.Net;

namespace Server
{

    // __CR__ [PSMG] Implementeer hier ook de IDisposable interface
    public class Server : IDisposable
    {
        private TcpListener patientListener;
        private TcpListener doctorListener;

        private Dictionary<string, ClientHandlerBase> clients;
        public DataManager manager { get; private set; }
        internal X509Certificate Certificate { get; private set; }

        public delegate void AddToList(ClientHandlerBase client);
        private AddToList addToList;
        public delegate void RemoveFromList(ClientHandlerBase client);
        private RemoveFromList removeFromList;

        private IConfiguration config;

        static void Main(string[] args)
        {
            //new Test();
            new Server();

        }


        public Server()
        {
            this.clients = new Dictionary<string, ClientHandlerBase>();
            // __CR__ [PSMG] Zou je de poort niet als een constant in het shared project zetten

            this.manager = new DataManager();
            this.addToList += AddClient;
            this.removeFromList += RemoveClient;
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddUserSecrets<Server>()
                .AddJsonFile(path: "config.json");
            config = builder.Build();

            this.Certificate = new X509Certificate(config["CertificatePath"], config["CertificatePassword"]);
            this.patientListener = new TcpListener(IPAddress.Any, 5005);

            patientListener.Start();
            patientListener.BeginAcceptTcpClient((ar) => OnConnect(ar, true), null);
            Console.WriteLine($"Listening for Patient connections on port: 5005");

            this.doctorListener = new TcpListener(IPAddress.Any, 6006);
            this.doctorListener.Start();
            this.doctorListener.BeginAcceptTcpClient((ar) => OnConnect(ar, false), null);
            Console.WriteLine($"Listening for Doctor connections on port: 6006");



            Console.Read();
        }

        public void AddClient(ClientHandler client) {
        if (!this.clients.ContainsKey(client.Name))
            this.clients.Add(client.Name , client);
		}

        public void RemoveClient(ClientHandlerBase client)
        {
            this.clients.Remove(client.Name);
        }

        private void OnConnect(IAsyncResult ar, bool isPatient)
        {
            TcpClient tcpClient = null;
            if (isPatient)
            {
                tcpClient = patientListener.EndAcceptTcpClient(ar);
                patientListener.BeginAcceptTcpClient((ar) => OnConnect(ar, isPatient), null);
            }
            else
            {
                tcpClient = doctorListener.EndAcceptTcpClient(ar);
                doctorListener.BeginAcceptTcpClient((ar) => OnConnect(ar, isPatient), null);
            }
            Console.WriteLine($"Client connected from {tcpClient.Client.RemoteEndPoint}");
            new PatientClientHandler(tcpClient, this, addToList, removeFromList);
        }


        public void send(Root message)
        {
            string target = message.Target;
            bool found = false;

            // __CR__ [PSMG] Eventueel omzetten naar enum
            if (target == "all")
            {
                found = true;
                foreach (ClientHandlerBase client in clients.Values)
                {
                    //TODO kijken dat het niet naar dokter wordt gestuurd
                    if (target != message.Sender)
                        client.send(message);
                }
            }
            else
            {
                ClientHandlerBase client = null;
                clients.TryGetValue(target, out client);
                if (client != null) {
                    found = true;
                    client.send(message);
                } else {
                    Console.WriteLine("Could not find client");
				}

            }

            if (!(message.Type == typeof(Acknowledge).FullName) && !(message.Type == typeof(HealthData).FullName))
            {
                if (!found)
                {
                    SendAcknowledge(message, 404, "target not found");
                }
                else
                {
                    SendAcknowledge(message, 200, "ok");
                }
            }
        }

        public void recieveClients(ref Root root)
        {
            List<string> clients = new List<string>();
            foreach (PatientClientHandler client in this.clients.Values)
            {
                if (client.Name != root.Sender)
                {
                    clients.Add(client.Name);
                }
            }
            root.Data = new Selection() { selection = clients };
        }

        public void SendAcknowledge(Root root, int status, string message)
        {
            this.send(new Root()
            {
                Sender = "server",
                Target = root.Sender,
                Type = typeof(Acknowledge).FullName,
                Data = new Acknowledge() { subtype = root.Type, status = status, statusmessage = message }
            });
        }

        public void Dispose()
        {
            foreach (PatientClientHandler client in this.clients.Values)
            {
                client.disconnect();
            }
            this.clients.Clear();
            this.patientListener.Stop();
        }
    }
}
