using CommunicationObjects.DataObjects;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    public class DoctorClientHandler : ClientHandlerBase
    {
        public DoctorClientHandler(TcpClient tcpClient, Server server, Server.AddToList add, Server.RemoveFromList remove) : base(tcpClient, server, add, remove)
        {
            start();
        }

        /// <summary>
        /// method for getting the client's name from the client
        /// </summary>
        /// <returns>the client's name</returns>
        private async Task<string> GetAuthorized()
        {
            string name = "";
            bool authorized = false;

            string message = await rw.Read();
            Root jsonObject = JsonConvert.DeserializeObject<Root>(message);

            if (jsonObject.Type == typeof(Connection).FullName &&
                (jsonObject.Data as JObject).ToObject<Connection>().connect)
            {
                Connection Data = (jsonObject.Data as JObject).ToObject<Connection>();
                name = jsonObject.Sender;
                Name = name;
                if (!Authorization.Authorized(name, Data.password))
                {
                    send(new Root
                    {
                        Type = typeof(Acknowledge).FullName,
                        Sender = "server",
                        Target = this.Name,
                        Data = new Acknowledge { subtype = typeof(Connection).FullName, status = 405, statusmessage = "Not Authorized." }
                    });

                    disconnect();

                }
                else
                {
                    this.addToList(this);
                    send(new Root
                    {
                        Type = typeof(Acknowledge).FullName,
                        Sender = "server",
                        Target = this.Name,
                        Data = new Acknowledge { subtype = typeof(Connection).FullName, status = 200, statusmessage = "Connection succesfull." }
                    });
                    authorized = true;
                }
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

            Type type = Type.GetType(root.Type);

            bool errorFound = false;

            if (type == typeof(Selection))
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
            else if (type == typeof(ClientsHistory))
            {
                string sender = root.Sender;
                root.Sender = sender;

                ClientsHistory data = new ClientsHistory();

                data.clients = DataManager.ReturnClientsFromInfoFolder();
                root.Data = data;

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
            else if (type == typeof(Session))
            {
                if (root.Sender == root.Target)
                {
                    this.server.SendAcknowledge(root, 409, "sender can't be target");
                    errorFound = true;
                }
            }

            if (this.active && !errorFound) this.server.send(root);
        }

        internal override async void start()
        {
            await GetAuthorized();
            if (!active)
            {
                Console.WriteLine("Doctor Failed to authorize");
                return;
            } else
            {
                Console.WriteLine("Doctor authorized");
                new Thread(base.Run).Start();
            }

        }
    }
}
