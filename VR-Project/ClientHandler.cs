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
            
            
        }

        public void StartConnection()
        {
            
            this.server = new Client(new TcpClient("localhost", 5005));
            HealthData data = new HealthData() { AccWatt = 100, CurWatt = 50, Heartbeat = 120, RPM = 95, Speed = 5.02 };
            Root dataRoot = new Root() { Type = typeof(HealthData).FullName, data = data, sender = "Henk", target = "Hank" };
            Root connectRoot = new Root() { Type = typeof(Connection).FullName, data = new Connection() { connect = true }, sender = "Henk", target = "server" };

            this.server.send(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(connectRoot)));

            while (true)
            {
                this.server.send(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(dataRoot)));
                
            }

        }

    }
}
