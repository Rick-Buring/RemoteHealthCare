using CommunicationObjects;
using CommunicationObjects.DataObjects;
using CommunicationObjects.util;
using DataStructures;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Vr_Project.RemoteHealthcare;

namespace VR_Project
{
	class ClientHandler
	{
		private Client client;
		private ViewModel.SendResistance resistanceUpdater;
		private bool active;
		private bool connected;

		private PriorityQueue<Message> queue;

		public ClientHandler(ViewModel.SendResistance resistanceUpdater)
		{
			this.resistanceUpdater = resistanceUpdater;
			this.connected = false;
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

		private void Parse(string toParse)
		{
			Root root = JsonConvert.DeserializeObject<Root>(toParse);

			Type type = Type.GetType(root.type);

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

		public void Update(Ergometer ergometer, HeartBeatMonitor heartBeatMonitor)
		{

			if (this.client != null)
			{

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
			}

		}

		public void Stop()
		{
			//TODO nullpointer afhandelen.
			if (this.client != null)
			{
				this.client.Write(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(new Root()
				{ type = typeof(Connection).FullName, data = new Connection() { connect = false }, sender = "Henk", target = "server" })));
				this.client.terminate();
			}
		}

	}
}
