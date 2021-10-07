using CommunicationObjects;
using CommunicationObjects.DataObjects;
using CommunicationObjects.util;
using DataStructures;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Media;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Vr_Project.RemoteHealthcare;

namespace VR_Project
{
	class ClientHandler
	{
		private Client client;
		public ViewModel.SendResistance resistanceUpdater { get; set; }
		private bool active;
		private bool connected;
		private bool isSessionRunning;

		private PriorityQueue<Message> queue;

		public ClientHandler(ViewModel.SendResistance resistanceUpdater)
		{
			this.resistanceUpdater = resistanceUpdater;
			this.connected = false;
			this.isSessionRunning = false;
			this.queue = new PriorityQueue<Message>(new MessageComparer());
			//this.queue.
		}

		public async void StartConnection()
		{

			this.client = new Client(new TcpClient("localhost", 5005));
			//Root connectRoot = new Root() { type = typeof(Connection).FullName, data = new Connection() { connect = true }, sender = "Henk", target = "server" };

			//this.client.Write(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(connectRoot)));
			//string connection = await this.client.Read();
			
			await GetConnection();
			if (this.connected)
			{
				await Run();
			} else {
				//do something to close connection
			}
			
			//When a disconnect message is send the Run method will end and here the conenction will be closed.
		}

		private async Task GetConnection () {
			Root connectRoot = new Root() { type = typeof(Connection).FullName, data = new Connection() { connect = true }, sender = "Henk", target = "server" };
			this.client.Write(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(connectRoot)));
			string connection = await this.client.Read();
			Parse(connection);
		}
		
		private async Task Run()
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
					this.active = false;
				}
			}
		}
		private SoundPlayer soundPlayer = new SoundPlayer(Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName, "mixkit-alert-alarm-1005.wav"));
		private void StopSound (Object source, ElapsedEventArgs e) {
			soundPlayer.Stop();
		}

		private void Parse(string toParse)
		{
			Root root = JsonConvert.DeserializeObject<Root>(toParse);

			Type type = Type.GetType(root.type);

			if (type == typeof(Setting))
			{
				Setting data = (root.data as JObject).ToObject<Setting>();

				if (data.emergencystop){
					this.resistanceUpdater(0);
					new Thread(async () => {
						soundPlayer.Load();
						soundPlayer.PlayLooping();
						
						System.Timers.Timer timer = new System.Timers.Timer(5000);
						timer.Elapsed += StopSound;
						timer.AutoReset = false;
						timer.Enabled = true;
						timer.Start();
					}).Start();
					//TODO stop bericht laten zien in chat en een geluid afspelen met SoundPlayer.
				} else {
					float targetResistance = data.res;
					this.resistanceUpdater(targetResistance);
					
					
				}

				//TODO notify vrclient dat de session start of stopt 
				if (data.sesionchange == SessionType.START) {
					this.isSessionRunning = true;
				} else if (data.sesionchange == SessionType.STOP){
					this.isSessionRunning = false;
				}
				

			}
			else if (type == typeof(Chat))
			{
				Chat data = (root.data as JObject).ToObject<Chat>();
				string message = data.message;

			}
			else if (type == typeof(Acknowledge))
			{
				Acknowledge ack = (root.data as JObject).ToObject<Acknowledge>();
				Type ackType = Type.GetType(ack.subtype);

				if (ackType == typeof(Connection))
				{
					this.connected = !this.connected;
					if (!this.connected) {
						this.active = false;
					}
				}

			}
		}

		private bool isLocked = false;
		public async void Update(Ergometer ergometer, HeartBeatMonitor heartBeatMonitor)
		{
			
			if (this.client != null && this.isSessionRunning && !this.isLocked)
			{
				this.isLocked = true;
				Root healthData = new Root()
				{
					type = typeof(HealthData).FullName,
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
				this.isLocked = false;
			}

		}

		public void Stop()
		{
			//TODO nullpointer afhandelen.
			if (this.client != null)
			{
				this.client.Write(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(new Root()
				{ type = typeof(Connection).FullName, data = new Connection() { connect = false }, sender = "Henk", target = "server" })));
				this.client.Dispose();
			}
		}

	}
}
