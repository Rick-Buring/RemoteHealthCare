using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using CommunicationObjects.DataObjects;

namespace Server
{
    public class Server
    {
        private TcpListener listener;
        private List<ClientHandler> clients;
        public DataManager manager { get; private set; }

        static void Main(string[] args)
        {
            new Test();
            //new Server();
        }

        public Server()
        {
            this.clients = new List<ClientHandler>();
            this.listener = new TcpListener(System.Net.IPAddress.Any, 5005);
            this.manager = new DataManager();

            listener.Start();
            listener.BeginAcceptTcpClient(new AsyncCallback(OnConnect), null);
        }

        private void OnConnect(IAsyncResult ar)
        {
            var tcpClient = listener.EndAcceptTcpClient(ar);
            Console.WriteLine($"Client connected from {tcpClient.Client.RemoteEndPoint}");
            clients.Add(new ClientHandler(tcpClient, this));
            listener.BeginAcceptTcpClient(new AsyncCallback(OnConnect), null);
        }

        public void send(Root message)
        {
            byte[] toSend = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(message));
            string target = message.target;

            if (target == "all")
            {
                foreach(ClientHandler client in clients)
                {
                    if (target != message.sender)
                    client.Client.send(toSend);
                }
                return;
            }
            foreach (ClientHandler client in clients)
            {
                if (target == client.Name)
                client.Client.send(toSend);
            }

        }

        public void recieveClients(ref Root root)
        {
            List<string> clients = new List<string>();
            foreach(ClientHandler client in this.clients)
            {
                clients.Add(client.Name);
            }
            root.data = new Selection() {selection = clients};
        }


    }
}
