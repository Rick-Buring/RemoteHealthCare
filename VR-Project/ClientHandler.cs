using CommunicationObjects;
using CommunicationObjects.DataObjects;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using Vr_Project.RemoteHealthcare;

namespace VR_Project
{
    class ClientHandler
    {
        private Client client;
        private ViewModel.SendResistance resistanceUpdater;
        

        public ClientHandler (ViewModel.SendResistance resistanceUpdater)
        {
            this.resistanceUpdater = resistanceUpdater;
        }

        public void StartConnection()
        {

            this.client = new Client(new TcpClient("localhost", 5005));
            Root connectRoot = new Root() { Type = typeof(Connection).FullName, data = new Connection() { connect = true }, sender = "Henk", target = "server" };

            this.client.Write(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(connectRoot)));

            
      
        }
        private bool active;
        private async void Run()
        {
            this.active = true;
            while (active)
            {
                try
                {
                    string result = await client.Read();
                    Console.WriteLine(result);
                    Parse(result);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    //todo disconnect client

                }
            }
        }
        
        private void Parse(string toParse)
        {
            Root root = JsonConvert.DeserializeObject<Root>(toParse);

            Type type = Type.GetType(root.Type);

            if (type == typeof(Setting))
            {
                Setting data = (root.data as JObject).ToObject<Setting>();
                float targetResistance = data.res;
                this.resistanceUpdater(targetResistance);

            }
            else if (type == typeof(Chat))
            {
                Chat data = (root.data as JObject).ToObject<Chat>();
                string message = data.message;

            }
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
            if (this.client != null)
            {
                this.client.Write(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(new Root()
                { Type = typeof(Connection).FullName, data = new Connection() { connect = false }, sender = "Henk", target = "server" })));
                this.client.terminate();
            }
        }

    }
}
