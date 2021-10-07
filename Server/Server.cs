using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using CommunicationObjects.DataObjects;
using System.Security.Cryptography.X509Certificates;

namespace Server
{

    // __CR__ [PSMG] Implementeer hier ook de IDisposable interface
    public class Server : IDisposable
    {
        private TcpListener listener;
        private List<ClientHandler> clients;
        public DataManager manager { get; private set; }
        internal X509Certificate Certificate { get; private set; }

        static void Main(string[] args)
        {
            //new Test();
            new Server();
        }

        public Server()
        {
            this.clients = new List<ClientHandler>();
            // __CR__ [PSMG] Zou je de poort niet als een constant in het shared project zetten
            this.listener = new TcpListener(System.Net.IPAddress.Any, 5005);
            this.manager = new DataManager();
            this.Certificate = new X509Certificate(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/Certificaat.pfx", "test1234");

            listener.Start();
            listener.BeginAcceptTcpClient(new AsyncCallback(OnConnect), null);

            Console.Read();
        }

        private void OnConnect(IAsyncResult ar)
        {
            listener.BeginAcceptTcpClient(new AsyncCallback(OnConnect), null);
            var tcpClient = listener.EndAcceptTcpClient(ar);
            Console.WriteLine($"Client connected from {tcpClient.Client.RemoteEndPoint}");
            clients.Add(new ClientHandler(tcpClient, this));
        }

        public void OnDisconnect(ClientHandler client)
        {
            // __CR__ [PSMG] Hier krijg je mogelijk een fout met multithreading!
            if (this.clients.Contains(client))
            {
                this.clients.Remove(client);
            }

        }

        public void send(Root message)
        {
            string target = message.target;
            bool found = false;

            // __CR__ [PSMG] Eventueel omzetten naar enum
            if (target == "all")
            {
                found = true;
                foreach (ClientHandler client in clients)
                {
                    if (target != message.sender)
                        client.send(message);
                }
            }
            else
            {
                foreach (ClientHandler client in clients)
                {
                    if (target == client.Name)
                    {
                        client.send(message);
                        found = true;
                    }
                }
            }

            if (!(message.type == typeof(Acknowledge).FullName) || !(message.type == typeof(HealthData).FullName))
            {
                if (found)
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
            foreach (ClientHandler client in this.clients)
            {
                if (client.Name != root.sender)
                {
                    clients.Add(client.Name);
                }
            }
            root.data = new Selection() { selection = clients };
        }

        public void SendAcknowledge(Root root, int status, string message)
        {
            this.send(new Root()
            {
                sender = "server",
                target = root.sender,
                type = typeof(Acknowledge).FullName,
                data = new Acknowledge() { subtype = root.type, status = status, statusmessage = message }
            });
        }

        public void Dispose()
        {
            foreach (ClientHandler client in this.clients)
            {
                client.disconnect();
            }
            this.listener.Stop();
        }
    }
}
