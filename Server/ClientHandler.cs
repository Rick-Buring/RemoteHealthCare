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
        public Client Client { get; private set; }

        private Server server;
        private bool active;

        /// <summary>
        /// Handles connecting clients
        /// </summary>
        /// <param name="tcpClient">The connecting client</param>
        /// <param name="server">The server the client is connecting too</param>
        public ClientHandler(TcpClient tcpClient, Server server)
        {
            this.Client = new Client(tcpClient);
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
                this.server.OnDisconnect(this);
                this.active = false;
            }
            return name;
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
                    Parse(result);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    //todo disconnect client
                }
            }
        }

        /// <summary>
        /// methot used to parse and react on messages
        /// </summary>
        /// <param name="toParse">message to be parsed must be of a json object type from the Root object</param>
        private void Parse(string toParse)
        {
            Root jsonObject = JsonConvert.DeserializeObject<Root>(toParse);

            string type = jsonObject.Type;

            switch (type)
            {
                case "Server.HealthData":
                    HealthData data = (jsonObject.data as JObject).ToObject<HealthData>();
                    this.server.manager.write(jsonObject.sender, data);
                    break;
                case "Server.Selection":
                    this.server.recieveClients(ref jsonObject);
                    break;
            }

            this.server.send(jsonObject);
        }


    }

}
