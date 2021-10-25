using CommunicationObjects;
using CommunicationObjects.DataObjects;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace DoktersApplicatie
{
	public class ClientHandler
	{
		private string name;
		private ReadWrite rw;
		private TcpClient client;
		private bool connected;
		private bool active;
		private ViewModel.ClientReceived addClient;
		private ViewModel.UpdateClient updateClient;
		private ViewModel.UpdateHistory updateHistory;

		public ClientHandler(ViewModel.ClientReceived addClient, ViewModel.UpdateClient updateClient, ViewModel.UpdateHistory updateHistory, string name)
		{
			this.name = name;
			this.connected = false;
			this.active = false;
			this.addClient = addClient;
			this.updateClient = updateClient;
			this.updateHistory = updateHistory;
		}

		public async Task StartConnection(string ip, int port)
		{
			this.client = new TcpClient(ip, port);
			SslStream stream = new SslStream(
					this.client.GetStream(),
					false,
					new RemoteCertificateValidationCallback(ReadWrite.ValidateServerCertificate),
					null
				);
			stream.AuthenticateAsClient(ReadWrite.certificateName);
			this.rw = new ReadWrite(stream);
			Root connectRoot = new Root() { Type = typeof(Connection).FullName, Data = new Connection() { connect = true, password = "test" }, Sender = this.name, Target = "server" };
			this.rw.Write(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(connectRoot)));
			Parse(await this.rw.Read());

			if (this.connected) await Run();
			
		}

		public async Task Run()
		{

			this.active = true;
			Root selectionRoot = new Root { Target = this.name, Sender = this.name, Type = typeof(Selection).FullName, Data = new Selection() };
			this.rw.Write(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(selectionRoot)));
			
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

		private void Parse(string toParse)
		{
			Root root = JsonConvert.DeserializeObject<Root>(toParse);

			Type type = Type.GetType(root.Type);

			if (type == typeof(Acknowledge))
			{
				Acknowledge ack = (root.Data as JObject).ToObject<Acknowledge>();

				Type ackType = Type.GetType(ack.subtype);
				if (ackType == typeof(Connection))
				{
					if (ack.status == 200) this.connected = !this.connected;
					Debug.WriteLine("Connected to server!!");
					//this.addClient(new Client {Name = });
					//this.isSessionRunning = !this.isSessionRunning;
					//this.connected = !this.connected;
					//if (!this.connected)
					//{
					//	this.active = false;
					//}
				} 


			} else if (type == typeof(History)) 
			{
				History history = (root.Data as JObject).ToObject<History>();
				this.updateHistory(history);
			}
			else if (type == typeof(HealthData))
			{
				HealthData healthData = (root.Data as JObject).ToObject<HealthData>();
				this.updateClient(new Client(root.Sender), healthData);
			} 
			else if (type == typeof(Selection)) 
			{
				Selection selection = (root.Data as JObject).ToObject<Selection>();
				foreach (string s in selection.selection) this.addClient(new Client(s));
			}
		}

		public void SetResistance(Client client, int resistance)
		{
			Root resistanceRoot = new Root { Sender = name, Target = client.Name, Type = typeof(Setting).FullName, Data = new Setting { res = resistance } };
			this.rw.Write(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(resistanceRoot)));
		}

		public async Task RequestHistory(Client client) {
			Root historyRoot = new Root { Sender = name, Target = client.Name, Type = typeof(History).FullName, Data = new History { clientName = client.Name } };
			this.rw.Write(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(historyRoot)));
		}
	}
}
