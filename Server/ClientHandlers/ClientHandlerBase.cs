using CommunicationObjects;
using CommunicationObjects.DataObjects;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public abstract class ClientHandlerBase : IDisposable
    {
        public string Name { get; set; }
        private readonly TcpClient Client;
        internal ReadWrite rw;
        internal Server server;
        internal bool active;
        internal Server.AddToList addToList;
        internal Server.RemoveFromList removeFromList;

        public ClientHandlerBase(TcpClient tcpClient, Server server, Server.AddToList add, Server.RemoveFromList remove)
        {
            this.Client = tcpClient;

            SslStream stream = new SslStream(this.Client.GetStream(), false);
            stream.AuthenticateAsServer(server.Certificate, clientCertificateRequired: false, checkCertificateRevocation: true);
            this.rw = new ReadWrite(stream);
            this.server = server;
            this.addToList = add;
            this.removeFromList = remove;
            this.active = true;
        }

        /// <summary>
        /// used to disconnect a this client from the server
        /// </summary>
        public void disconnect()
        {
            removeFromList(this);
            this.active = false;
            Dispose();
        }

        internal async void send(Root message)
        {
            byte[] toSend = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(message));
            await rw.Write(toSend);
        }

        /// <summary>
        /// function to handle incoming messages
        /// </summary>
        internal async void Run()
        {
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
        internal abstract void Parse(string toParse);
        internal abstract void start();

        public virtual void Dispose()
        {
            this.rw.Dispose();
            this.Client.Dispose();
        }
    }
}
