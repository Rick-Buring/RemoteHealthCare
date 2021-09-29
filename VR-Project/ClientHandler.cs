using CommunicationObjects;
using CommunicationObjects.DataObjects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Timers;
using Vr_Project.RemoteHealthcare;

namespace VR_Project
{
    class ClientHandler
    {
        private Client client;

        public void StartConnection()
        {

            this.client = new Client(new TcpClient("localhost", 5005));
            HealthData data = new HealthData() { AccWatt = 100, CurWatt = 50, Heartbeat = 120, RPM = 95, Speed = 5.02 };
            Root connectRoot = new Root() { Type = typeof(Connection).FullName, data = new Connection() { connect = true }, sender = "Henk", target = "server" };

            this.client.Write(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(connectRoot)));
      
        }

        public void Update(Ergometer ergometer, HeartBeatMonitor heartBeatMonitor)
        {

            if (this.client != null)
            {

                Root healthData = new Root()
                {
                    Type = typeof(HealthData).FullName,
                    data = new HealthData()
                    {
                        RPM = ergometer.GetErgometerData().Cadence,
                        AccWatt = ergometer.GetErgometerData().AccumulatedPower,
                        CurWatt = ergometer.GetErgometerData().InstantaneousPower,
                        Speed = ergometer.GetErgometerData().InstantaneousSpeed,
                        Heartbeat = heartBeatMonitor.GetHeartBeat()
                    },
                    sender = "Henk",
                    target = "Hank"
                };

                this.client.Write(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(healthData)));
            }

        }

        public void Stop()
        {
            //TODO nullpointer afhandelen.
            this.client.Write(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(new Root()
            { Type = typeof(Connection).FullName, data = new Connection() { connect = false }, sender = "Henk", target = "server" })));
            this.client.terminate();
        }

    }
}
