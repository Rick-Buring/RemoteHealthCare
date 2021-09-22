using System;
using System.Collections.Generic;
using System.Net.Sockets;
using Newtonsoft.Json;

namespace Server
{
    class Server
    {
        private TcpListener listener;
        private List<ClientHandler> clients;

        static void Main(string[] args)
        {
            new Server();
        }

        public Server()
        {
            this.listener = new TcpListener(System.Net.IPAddress.Any, 5005);
            listener.Start();
            listener.BeginAcceptTcpClient(new AsyncCallback(OnConnect), null);
        }

        private void OnConnect(IAsyncResult ar)
        {
            var tcpClient = listener.EndAcceptTcpClient(ar);
            Console.WriteLine($"Client connected from {tcpClient.Client.RemoteEndPoint}");
            clients.Add(new ClientHandler(tcpClient));
            listener.BeginAcceptTcpClient(new AsyncCallback(OnConnect), null);
        }
    }
}
