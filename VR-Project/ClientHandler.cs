using CommunicationObjects;
using CommunicationObjects.DataObjects;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using Vr_Project.RemoteHealthcare;

namespace VR_Project
{
    class ClientHandler : BindableBase, INotifyPropertyChanged
    {
        private ReadWrite rw;
        private TcpClient client;

        public string PatientName { get; set; } = "Patient Name";
        public void StartConnection(string ip, int port)
        {
            if (this.client != null)
                throw new ArgumentException("The Client is already connected dispose this first");
            this.client = new TcpClient(ip, port);

            SslStream stream = new SslStream(
                this.client.GetStream(),
                false,
                new RemoteCertificateValidationCallback(ReadWrite.ValidateServerCertificate),
                null
            );
            stream.AuthenticateAsClient(ReadWrite.certificateName);
            
            this.rw = new ReadWrite(stream);
            Root connectRoot = new Root() { Type = typeof(Connection).FullName, data = new Connection() { connect = true }, sender = "Henk", target = "server" };

            this.rw.Write(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(connectRoot)));
        }
        private bool active;
        private async void Run()
        {
            this.active = true;
            while (active)
            {
                try
                {
                    string result = await rw.Read();
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
                if (ViewModel.resistanceUpdater != null)
                    ViewModel.resistanceUpdater(targetResistance);

            }
            else if (type == typeof(Chat))
            {
                Chat data = (root.data as JObject).ToObject<Chat>();
                string message = data.message;

            }
        }

        public void Update(Ergometer ergometer, HeartBeatMonitor heartBeatMonitor)
        {

            if (this.client != null && this.rw != null)
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

                this.rw.Write(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(healthData)));
            }

        }

        public void Stop()
        {
            //TODO nullpointer afhandelen.
            if (this.rw != null)
            {
                this.rw.Write(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(new Root()
                { Type = typeof(Connection).FullName, data = new Connection() { connect = false }, sender = "Henk", target = "server" })));
                this.rw.terminate();
            }
        }

    }
}
