using CommunicationObjects;
using CommunicationObjects.DataObjects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

using System.Net.Sockets;
using System.Text;

namespace VR_Project
{
    class ClientHandler
    {

        private Client server;
        

        public ClientHandler ()
        {

            this.running = true;
            this.stopped = false;
        }

        private bool running;
        private bool stopped;
        public void StartConnection()
        {
            
            this.server = new Client(new TcpClient("localhost", 5005));
            HealthData data = new HealthData() { AccWatt = 100, CurWatt = 50, Heartbeat = 120, RPM = 95, Speed = 5.02 };
            Root dataRoot = new Root() { Type = typeof(HealthData).FullName, data = data, sender = "Henk", target = "Hank" };
            Root connectRoot = new Root() { Type = typeof(Connection).FullName, data = new Connection() { connect = true }, sender = "Henk", target = "server" };

            this.server.Write(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(connectRoot)));

            while (this.running)
            {
                this.server.Write(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(dataRoot)));
                
            }

            this.server.Write(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(new Root() 
            { Type = typeof(Connection).FullName, data = new Connection() { connect = false }, sender = "Henk", target = "server" })));
            this.server.terminate();
            this.stopped = true;

        }

        public void stop ()
        {
            this.running = false;
        }

        public bool isStopped ()
        {
            return this.stopped;
        }

    }
}
