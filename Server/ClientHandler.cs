using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using CommunicationObjects.DataObjects;
using CommunicationObjects;

namespace Server
{
    public class ClientHandler
    {
        public string Name { get; }
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

            this.Name = getName();

            new Thread(Run).Start();
        }

        private string getName()
        {
            string message = Client.Read();
            string name = "";
            Root jsonObject = JsonConvert.DeserializeObject<Root>(message);

            if (jsonObject.Type == typeof(Connection).FullName &&
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

        private void disconnect()
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
        private void Run()
        {
            this.active = true;
            while (active)
            {
                try
                {
                    string result = Client.Read();
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
                    string sender = root.sender;
                    root.target = root.sender;
                    root.sender = sender;

                    send(root);

                    this.disconnect();
                }
            }
            else if (false) //todo change false to type == typeof(historische informatie opvragen type)
            {
                Root returnRoot = new Root();

                returnRoot.sender = root.target;
                returnRoot.target = root.sender;

                root = returnRoot;
            }

            this.server.send(root);
        }


    }

}
