
using CommunicationObjects;
using CommunicationObjects.DataObjects;
using DataStructures;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Media;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Timers;
using System.Windows.Documents;
using Vr_Project.RemoteHealthcare;
using VR_Project.ViewModels;

namespace VR_Project
{
	public class ClientHandler : BindableBase, INotifyPropertyChanged, IDisposable
    {
        private ReadWrite rw;
        private TcpClient client;
		private bool active;
		private bool connected;
        public ConnectedVM.RequestResistance resistanceUpdater { get; set; }
        public ConnectedVM.SendChatMessage sendChat { get; set; }

        public ClientHandler()
        {
            ViewModel.updater += Update;
            this.sessionTargets = new List<string>();
        }

		public string PatientName { get; set; } = "Patient Name";
        private PriorityQueue<CommunicationObjects.util.PriortyQueueMessage> queue;
        public async void StartConnection(string ip, int port)
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

            
            Root connectRoot = new Root() { Type = typeof(Connection).FullName, Data = new Connection() { connect = true }, Sender = PatientName, Target = "server" };
            this.rw.Write(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(connectRoot)));
            Parse(await this.rw.Read());

            if (this.active)
            {
                Run();
            }
            else
            {

            }

        }

        private async void Run()
        {
            this.active = true;

            //await this.rw.Read();
            //this.isSessionRunning = true;
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
                    this.active = false;

                }
            }
        }
        private SoundPlayer soundPlayer = new SoundPlayer(Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName, "mixkit-alert-alarm-1005.wav"));
        private void StopSound(Object source, ElapsedEventArgs e)
        {
            soundPlayer.Stop();
        }

        private void Parse(string toParse)
        {
            Root root = JsonConvert.DeserializeObject<Root>(toParse);

            Type type = Type.GetType(root.Type);

            if (type == typeof(Setting))
            {
                Setting data = (root.Data as JObject).ToObject<Setting>();

                if (data.emergencystop)
                {
                    this.resistanceUpdater(0);
                    new Thread(async () =>
                    {
                        soundPlayer.Load();
                        soundPlayer.PlayLooping();

                        System.Timers.Timer timer = new System.Timers.Timer(5000);
                        timer.Elapsed += StopSound;
                        timer.AutoReset = false;
                        timer.Enabled = true;
                        timer.Start();
                    }).Start();
                    //TODO stop bericht laten zien in chat en een geluid afspelen met SoundPlayer.
                }
                else
                {
                    float targetResistance = data.res;
                    this.resistanceUpdater(targetResistance);


                }

            }
            else if (type == typeof(Chat))
            {
                Chat data = (root.Data as JObject).ToObject<Chat>();
                string message = data.message;
                this.sendChat(message);
                //ViewModels.ConnectedVM.AddMessage(new CommunicationObjects.DataObjects.Message() { Receiver = root.Target, Sender = root.Sender, Text = data.message });
            }
            else if (type == typeof(Acknowledge))
            {
                Acknowledge ack = (root.Data as JObject).ToObject<Acknowledge>();
                Type ackType = Type.GetType(ack.subtype);
                if (ackType == typeof(Connection))
                {
                    this.connected = !this.connected;
                    if (!this.connected)
                    {
                        this.active = false;
                    }
                }

            }
            else if (type == typeof(Session))
            {
                changeSessionClients(root.Sender);
            }
        }

        private void changeSessionClients(string client)
        {
            if (sessionTargets.Contains(client))
            {
                sessionTargets.Remove(client);
            }
            else
            {
                sessionTargets.Add(client);
            }
        }

        private List<String> sessionTargets;
		private bool isLocked = false;
		public async void Update(Ergometer ergometer, HeartBeatMonitor heartBeatMonitor)
		{
			
			if (this.client != null && this.sessionTargets.Count > 0 && !this.isLocked)
			{
				this.isLocked = true;
				ErgometerData data = ergometer.GetErgometerData();
				Root healthData = new Root()
				{
					Type = typeof(HealthData).FullName,
					Data = new HealthData()
					{
						RPM = data.Cadence,
						AccWatt = data.AccumulatedPower,
						CurWatt = data.InstantaneousPower,
						Speed = data.InstantaneousSpeed,
						Heartbeat = heartBeatMonitor.GetHeartBeat(),
						ElapsedTime = data.ElapsedTime,
						DistanceTraveled = data.DistanceTraveled
					},
					Sender = this.PatientName
                };
                foreach (string target in sessionTargets)
                {
                    healthData.Target = target;
                    this.rw.Write(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(healthData)));
                }
				
				this.isLocked = false;
            }
        }
        public void Stop()
        {
            //TODO nullpointer afhandelen.
            if (this.rw != null)
            {
                this.rw.Write(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(new Root()
                { Type = typeof(Connection).FullName, Data = new Connection() { connect = false }, Sender = "henk", Target = "server" })));
                this.rw.Dispose();
            }
        }

        public void Dispose()
        {
            this.rw.Dispose();
            this.client.Dispose();
        }
    }
}
