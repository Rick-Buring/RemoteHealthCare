using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Server.DataObjects;

namespace Server
{
    public class ClientHandler
    {
        public string Name { get; }

        private TcpClient tcpClient;
        private NetworkStream stream;
        private Server server;

        /// <summary>
        /// Handles connecting clients
        /// </summary>
        /// <param name="tcpClient">The connecting client</param>
        /// <param name="server">The server the client is connecting too</param>
        public ClientHandler(TcpClient tcpClient, Server server)
        {
            this.tcpClient = tcpClient;
            this.stream = this.tcpClient.GetStream();
            this.server = server;
            this.Name = Read();

            new Thread(Run).Start();
        }

        /// <summary>
        /// function to handle incoming messages
        /// </summary>
        private void Run()
        {
            while (true)
            {
                try
                {
                    string result = Read();
                    Parse(result);
                }catch(Exception e)
                {
                    Console.WriteLine(e);
                    //todo disconnect client
                }
            }
        }

        /// <summary>
        /// Reads incomming messages
        /// </summary>
        /// <returns>message in form of a string</returns>
        private string Read()
        {
            byte[] length = new byte[4];
            this.stream.Read(length, 0, 4);

            int size = BitConverter.ToInt32(length);

            byte[] received = new byte[size];

            int bytesRead = 0;
            while (bytesRead < size)
            {
                int read = this.stream.Read(received, bytesRead, received.Length - bytesRead);
                bytesRead += read;
                Console.WriteLine("ReadMessage: " + read);
            }

            return Encoding.ASCII.GetString(received);
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

        /// <summary>
        /// send messages to the client
        /// </summary>
        /// <param name="message">message to be sent to the client</param>
        internal void send(byte[] message)
        {
            stream.Write(message);
            stream.Flush();
        }
    }

}
