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

namespace Server
{
    public class ClientHandler
    {
        public string Name { get; set; }
        private Client Client { get; }

        private Server server;
        private bool active;

        /// <summary>
        /// Handles connecting clients
        /// </summary>
        /// <param name="tcpClient">The connecting client</param>
        /// <param name="server">The server the client is connecting too</param>
        public ClientHandler(TcpClient tcpClient, Server server)
        {
            this.Client = new Client(tcpClient, server.Certificate);
            this.server = server;

            

            new Thread(Run).Start();
        }

        /// <summary>
        /// method for getting the client's name from the client
        /// </summary>
        /// <returns>the client's name</returns>
        private async Task<string> getName()
        {
            string message = await Client.Read();
            string name = "";
            Root jsonObject = JsonConvert.DeserializeObject<Root>(message);

            if (jsonObject.type == typeof(Connection).FullName &&
                (jsonObject.data as JObject).ToObject<Connection>().connect)
            {
                name = jsonObject.sender;
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
            this.Client.terminate();
        }

        internal void send(Root message)
        {
            byte[] toSend = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(message));
            Client.Write(toSend);
        }

        /// <summary>
        /// function to handle incoming messages
        /// </summary>
        private async void Run()
        {
            this.Name = await getName();
            send(new Root { type = typeof(Acknowledge).FullName,
                data = new Acknowledge { subtype = typeof(Connection).FullName, status = 200, statusmessage = "Connection succesfull."},
                sender = "server", target = this.Name });
            //send acknowledgement
            
            this.active = true;
            while (active)
            {
                try
                {
                    string result = await Client.Read();
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

            Type type = Type.GetType(root.type);

            bool errorFound = false;

            if (type == typeof(HealthData))
            {
                HealthData data = (root.data as JObject).ToObject<HealthData>();
                this.server.manager.write(root.sender, data);
            }
            else if (type == typeof(Selection))
            {
                this.server.recieveClients(ref root);
            }
            else if (type == typeof(Connection))
            {
                if (!(root.data as JObject).ToObject<Connection>().connect)
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
                string sender = root.sender;
                root.target = root.sender;
                root.sender = sender;

                History data = (root.data as JObject).ToObject<History>();

                data.clientHistory = this.server.manager.GetHistory(data.clientName);

                root.data = data;
            }
            else if (type == typeof(Setting))
            {
                Setting data = (root.data as JObject).ToObject<Setting>();
                if ((data.res > 100 || data.res < 0) && !data.emergencystop)
                {
                    this.server.SendAcknowledge(root, 412, "invalid resistance value");
                    errorFound = true;
                }
            }
            else if (type == typeof(Chat))
            {
                if (root.sender == root.target)
                {
                    this.server.SendAcknowledge(root, 409, "sender can't be target");
                    errorFound = true;
                }

                Chat data = (root.data as JObject).ToObject<Chat>();
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
