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
    public class PatientClientHandler : ClientHandlerBase
    {
        public PatientClientHandler(TcpClient tcpClient, Server server, Server.AddToList add, Server.RemoveFromList remove) : base(tcpClient, server, add, remove)
        {
            start();
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
        /// methot used to parse and react on messages
        /// </summary>
        /// <param name="toParse">message to be parsed must be of a json object type from the Root object</param>
        internal override void Parse(string toParse)
        {
            Root root = JsonConvert.DeserializeObject<Root>(toParse);

            if (root == null)
            {
                return;
            }

            Type type = Type.GetType(root.Type);

            bool errorFound = false;

            if (type == typeof(HealthData))
            {
                HealthData data = (root.Data as JObject).ToObject<HealthData>();
                this.server.manager.write(root.Sender, data);
            }
            else if (type == typeof(Selection))
            {
                this.server.ReceiveClients(ref root);
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

        internal override async void start()
        {
            await getName();
            if (!active)
            {
                Console.WriteLine("Client Failed protocol");
                return;
            }
            Console.WriteLine("Client connected");
            new Thread(base.Run).Start();
        }
    }
}
