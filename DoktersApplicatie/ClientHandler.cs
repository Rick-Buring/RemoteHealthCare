using CommunicationObjects;
using CommunicationObjects.DataObjects;
using DoktersApplicatie.ViewModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Security;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace DoktersApplicatie
{
    public class ClientHandler : IDisposable
    {
        private string name;
        private ReadWrite rw;
        private TcpClient client;
        private bool connected;
        private bool active;
        private HomeVM.ClientReceived addClients;
        private HomeVM.UpdateClient updateClient;
        private HomeVM.UpdateHistory updateHistory;
        private bool loggedIn;

        public string[] clients { get; set; }

        public ClientHandler()
        {
            loggedIn = false;
        }

        public void addDelegates(HomeVM.ClientReceived addClient, HomeVM.UpdateClient updateClient,
            HomeVM.UpdateHistory updateHistory)
        {
            this.connected = false;
            this.active = false;
            this.addClients = addClient;
            this.updateClient = updateClient;
            this.updateHistory = updateHistory;
        }

        public async Task StartConnection(string ip, int port)
        {
            this.client = new TcpClient();
            await this.client.ConnectAsync(ip, port);
            SslStream stream = new SslStream(
                this.client.GetStream(),
                false,
                new RemoteCertificateValidationCallback(ReadWrite.ValidateServerCertificate),
                null
            );
            stream.AuthenticateAsClient(ReadWrite.certificateName);
            this.rw = new ReadWrite(stream);
        }

        public async Task<bool> Login(string name, string password)
        {
            this.name = name;
            Root connectRoot = new Root()
            {
                Type = typeof(Connection).FullName,
                Data = new Connection() { connect = true, password = password },
                Sender = name,
                Target = "server"
            };
            await this.rw.Write(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(connectRoot)));
            Root r = JsonConvert.DeserializeObject<Root>(await this.rw.Read());
            this.loggedIn = (r.Data as JObject).ToObject<Acknowledge>().status == 200;

            return loggedIn;
        }



        public async Task Run()
        {

            this.active = true;
            Root selectionRoot = new Root
            {
                Target = this.name,
                Sender = this.name,
                Type = typeof(Selection).FullName,
                Data = new Selection()
            };
            await this.rw.Write(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(selectionRoot)));

            while (active)
            {
                try
                {
                    string result = await rw.Read();
                    Debug.WriteLine(result);
                    Parse(result);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.StackTrace);
                    this.active = false;

                }
            }
        }

        private HistoryVM historyVm { get; set; }


        private void Parse(string toParse)
        {
            Root root = JsonConvert.DeserializeObject<Root>(toParse);

            if (root == null)
            {
                return;
            }

            Type type = Type.GetType(root.Type);

            if (type == typeof(Acknowledge))
            {
                Acknowledge ack = (root.Data as JObject).ToObject<Acknowledge>();

                Type ackType = Type.GetType(ack.subtype);
                if (ackType == typeof(Connection))
                {
                    if (ack.status == 200)
                    {
                        this.connected = !this.connected;
                        if (!this.connected)
						{
                            this.active = false;
                            this.rw.Dispose();
                            this.client.GetStream().Close();
                            this.client.GetStream().Dispose();
                            this.client.Close();
                            this.client.Dispose();
                        }
                        Debug.WriteLine("Connected to server!!");
                    }
                }


            }
            else if (type == typeof(History))
            {
                History history = (root.Data as JObject).ToObject<History>();
                historyVm.HistoryData = new HistoryData(history);
            }
            else if (type == typeof(ClientsHistory))
            {
                ClientsHistory clientsHistory = (root.Data as JObject).ToObject<ClientsHistory>();
                if (clientsHistory.clients != null)
                {
                    Debug.WriteLine("Clients: " + String.Join(", ", clientsHistory.clients));
                }

                historyVm = new HistoryVM(clientsHistory.clients, this);

            }
            else if (type == typeof(HealthData))
            {
                HealthData healthData = (root.Data as JObject).ToObject<HealthData>();
                this.updateClient(root.Sender, healthData);
            }
            else if (type == typeof(Selection))
            {
                Selection selection = (root.Data as JObject).ToObject<Selection>();
                List<Client> list = new List<Client>();
                foreach (string s in selection.selection) list.Add(new Client(s));
                this.addClients(list);
            }
        }

        public async Task SetResistance(Client client, int resistance)
        {
            Root resistanceRoot = new Root
            {
                Sender = name,
                Target = client.Name,
                Type = typeof(Setting).FullName,
                Data = new Setting { res = resistance }
            };
            await this.rw.Write(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(resistanceRoot)));
        }

        public async Task RequestHistory(History client)
        {
            Root historyRoot = new Root
            {
                Sender = name,
                Target = client.clientName,
                Type = typeof(History).FullName,
                Data = client
            };
           await this.rw.Write(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(historyRoot)));
        }

        public async Task RequestClientsHistory()
        {
            Root clientsHistory = new Root
            {
                Sender = name,
                Target = name,
                Type = typeof(ClientsHistory).FullName,
                Data = new ClientsHistory()
            };
          await this.rw.Write(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(clientsHistory)));
        }

        public async void SendChat(bool all, string message, Client client = null)
        {
            Root chatRoot = new Root
            {
                Sender = name,
                Type = typeof(Chat).FullName,
                Data = new Chat { message = message }
            };

            if (all)
            {
                chatRoot.Target = "all";
            }
            else
            {
                if (client == null) return;
                chatRoot.Target = client.Name;
            }

           await this.rw.Write(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(chatRoot)));
        }

        public async void EmergencyStop(bool all, Client client = null)
        {
            if (all)
            {

                Root emergencyRoot = new Root
                {
                    Sender = name,
                    Target = "all",
                    Type = typeof(Setting).FullName,
                    Data = new Setting { res = 0, emergencystop = true }
                };
              await this.rw.Write(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(emergencyRoot)));
            }
            else
            {
                Root emergencyRoot = new Root
                {
                    Sender = name,
                    Target = client.Name,
                    Type = typeof(Setting).FullName,
                    Data = new Setting { res = 0, emergencystop = true }
                };
               await this.rw.Write(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(emergencyRoot)));
            }
        }

        public async void StartStopSession(Client client)
        {
            Root emergencyRoot = new Root
            {
                Sender = name,
                Target = client.Name,
                Type = typeof(Session).FullName,
                Data = new Session()
            };
           await this.rw.Write(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(emergencyRoot)));
        }

        public async void Stop()
        {
          await this.rw.Write(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(new Root { Sender = name, Target = "server", Type = typeof(Connection).FullName, Data = new Connection { connect = false } })));
        }

		public void Dispose()
		{
            Stop();
		}
	}
}




