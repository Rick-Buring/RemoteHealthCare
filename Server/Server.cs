using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using CommunicationObjects.DataObjects;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace Server
{

    // __CR__ [PSMG] Implementeer hier ook de IDisposable interface
    public class Server : IDisposable
    {
        private TcpListener listener;
        private Dictionary<string,ClientHandler> clients;
        public DataManager manager { get; private set; }
        internal X509Certificate Certificate { get; private set; }

        public delegate void AddToList(ClientHandler client);
        private AddToList addToList;
        public delegate void RemoveFromList(ClientHandler client);
        private RemoveFromList removeFromList;

        private IConfiguration config;

        static void Main(string[] args)
        {
            //new Test();
            new Server();
            
        }


        public Server()
        {
            this.clients = new Dictionary<string, ClientHandler>();
            // __CR__ [PSMG] Zou je de poort niet als een constant in het shared project zetten
            this.listener = new TcpListener(System.Net.IPAddress.Any, 5005);
            this.manager = new DataManager();
            this.addToList += AddClient;
            this.removeFromList += RemoveClient;
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddUserSecrets<Server>()
                .AddJsonFile(path: "config.json");
            config = builder.Build();

            this.Certificate = new X509Certificate(config["CertificatePath"], config["CertificatePassword"]);


            listener.Start();
            listener.BeginAcceptTcpClient(new AsyncCallback(OnConnect), null);

            Console.Read();
        }

        public async void AddClient(ClientHandler client) {
        if (!this.clients.ContainsKey(client.Name))
            this.clients.Add(client.Name , client);
		}

        public async void RemoveClient(ClientHandler client) {
            this.clients.Remove(client.Name);
		}

        private void OnConnect(IAsyncResult ar)
        {
            var tcpClient = listener.EndAcceptTcpClient(ar);
            listener.BeginAcceptTcpClient(new AsyncCallback(OnConnect), null);
            Console.WriteLine($"Client connected from {tcpClient.Client.RemoteEndPoint}");
            //clients.Add(new ClientHandler(tcpClient, this));
            new ClientHandler(tcpClient, this, addToList, removeFromList);
        }

        public void OnDisconnect(ClientHandler client)
        {
			// __CR__ [PSMG] Hier krijg je mogelijk een fout met multithreading!
			//if (this.clients.Contains(client))
			//{
			//	this.clients.Remove(client);
			//}

		}

        public void send(Root message)
        {
            string target = message.Target;
            bool found = false;

            // __CR__ [PSMG] Eventueel omzetten naar enum
            if (target == "all")
            {
                found = true;
                foreach (ClientHandler client in clients.Values)
                {
                //TODO kijken dat het niet naar dokter wordt gestuurd
                    if (target != message.Sender)
                        client.send(message);
                }
            }
            else
            {
                ClientHandler client = null;
                clients.TryGetValue(target, out client);
                if (client != null) {
                    client.send(message);
                } else {
                    Console.WriteLine("Could not find client");
				}
                  

                //foreach (ClientHandler client in clients.Values)
                //{
                //    if (target == client.Name)
                //    {
                //        client.send(message);
                //        found = true;
                //    }
                //}
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
            foreach (ClientHandler client in this.clients.Values)
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
            foreach (ClientHandler client in this.clients.Values)
            {
                client.disconnect();
            }
            this.clients.Clear();
            this.listener.Stop();
        }
    }
}
