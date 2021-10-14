using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using CommunicationObjects.DataObjects;
using CommunicationObjects;
using System.Net.Security;

namespace Server
{
    public class ClientHandler
    {
        public string Name { get; set; }
        private TcpClient Client { get; }
        private ReadWrite rw;

        private Server server;
        private bool active;

        private Server.AddToList addToList;
        private Server.RemoveFromList removeFromList;

		/// <summary>
		/// Handles connecting clients
		/// </summary>
		/// <param name="tcpClient">The connecting client</param>
		/// <param name="server">The server the client is connecting too</param>
		public ClientHandler(TcpClient tcpClient, Server server, Server.AddToList add, Server.RemoveFromList remove)
        {
            this.Client = tcpClient;

            SslStream stream = new SslStream(this.Client.GetStream(), false);
            stream.AuthenticateAsServer(server.Certificate, clientCertificateRequired: false, checkCertificateRevocation: true);
            this.rw = new ReadWrite(stream);
            this.server = server;
            this.addToList = add;
            this.removeFromList = remove;
            

            new Thread(Run).Start();
        }

        /// <summary>
        /// method for getting the client's name from the client
        /// </summary>
        /// <returns>the client's name</returns>
        private async Task<string> getName()
        {
            string message = await rw.Read();
            string name = "";
            Root jsonObject = JsonConvert.DeserializeObject<Root>(message);

            if (jsonObject.Type == typeof(Connection).FullName &&
                (jsonObject.Data as JObject).ToObject<Connection>().connect)
            {
                name = jsonObject.Sender;
                Name = name;
                this.addToList(this);
            }
            else
            {
                disconnect();
            }
            return name;
        }

        /// <summary>
        /// used to disconnect a this client from the server
        /// </summary>
        public void disconnect()
        {
            this.server.OnDisconnect(this);
            this.active = false;
            this.rw.terminate();
        }

        internal void send(Root message)
        {
            byte[] toSend = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(message));
            rw.Write(toSend);
        }

        /// <summary>
        /// function to handle incoming messages
        /// </summary>
        private async void Run()
        {
            this.Name = await getName();
            send(new Root
            {
                Type = typeof(Acknowledge).FullName,
                Sender = "server",
                Target = this.Name,
                Data = new Acknowledge { subtype = typeof(Connection).FullName, status = 200, statusmessage = "Connection succesfull." }
            });
            //send acknowledgement
            //Temporary doctor command
            send(new Root { Type = typeof(Setting).FullName, Sender = "server", Target = this.Name, Data = new Setting { emergencystop = false, res = 50, sesionchange = SessionType.START } });
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
                    this.disconnect();
                }
            }
        }

        /// <summary>
        /// methot used to parse and react on messages
        /// </summary>
        /// <param name="toParse">message to be parsed must be of a json object type from the Root object</param>
        private void Parse(string toParse)
        {
            Root root = JsonConvert.DeserializeObject<Root>(toParse);

            Type type = Type.GetType(root.Type);

            bool errorFound = false;

            if (type == typeof(HealthData))
            {
                HealthData data = (root.Data as JObject).ToObject<HealthData>();
                this.server.manager.write(root.Sender, data);
            }
            else if (type == typeof(Selection))
            {
                this.server.recieveClients(ref root);
            }
            else if (type == typeof(Connection))
            {
                if (!(root.Data as JObject).ToObject<Connection>().connect)
                {
                    this.server.SendAcknowledge(root, 200, "terminating connection");
                    this.disconnect();
                }
                else
                {
                    this.server.SendAcknowledge(root, 403, "already connected");
                    errorFound = true;
                }
            }
            else if (type == typeof(History))
            {
                string sender = root.Sender;
                root.Target = root.Sender;
                root.Sender = sender;

                History data = (root.Data as JObject).ToObject<History>();

                data.clientHistory = this.server.manager.GetHistory(data.clientName);

                root.Data = data;
            }
            else if (type == typeof(Setting))
            {
                Setting data = (root.Data as JObject).ToObject<Setting>();
                if ((data.res > 100 || data.res < 0) && !data.emergencystop)
                {
                    this.server.SendAcknowledge(root, 412, "invalid resistance value");
                    errorFound = true;
                }
            }
            else if (type == typeof(Chat))
            {
                if (root.Sender == root.Target)
                {
                    this.server.SendAcknowledge(root, 409, "sender can't be target");
                    errorFound = true;
                }

                Chat data = (root.Data as JObject).ToObject<Chat>();
                if (data.message == "" && !errorFound)
                {
                    this.server.SendAcknowledge(root, 412, "empty messages are not allowed");
                    errorFound = true;
                }
            }

            if (this.active && !errorFound) this.server.send(root);
        }
    }
}
