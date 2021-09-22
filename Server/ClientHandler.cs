using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Server
{
    public class ClientHandler
    {
        private TcpClient tcpClient;
        private NetworkStream stream;
        private Server server;
        public string name { private set; get; }

        public ClientHandler(TcpClient tcpClient, Server server)
        {
            this.tcpClient = tcpClient;
            this.stream = this.tcpClient.GetStream();
            this.server = server;

            new Thread(Read).Start();
        }

        private void Read()
        {
            for(;;)
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

                string result = Encoding.ASCII.GetString(received);
                Parse(result);
            }
        }

        private void Parse(string toParse)
        {
            Root jsonObject = JsonConvert.DeserializeObject<Root>(toParse);

            string type = jsonObject.Type;

            switch (type)
            {
                case "Server.HealthData":
                    HealthData data = ((JObject)jsonObject.data).ToObject<HealthData>();
                    this.server.manager.write(jsonObject.sender, data);
                    break;
                case "Server.Selection":
                    this.server.recieveClients(ref jsonObject);
                    break;
            }

            this.server.send(jsonObject);
        }

        internal void send(byte[] message)
        {
            stream.Write(message);
            stream.Flush();
        }
    }

    public class Test
    {
        private string test = "{   \"Type\":\"Server.Chat\",   \"sender\":\"doc\",   \"data\":{      \"message\":\"hallo\"   },   \"target\":\"henk\"}";

        public Test()
        {
            Root oobject = JsonConvert.DeserializeObject<Root>(test);

            //Type ts = Type.GetType(oobject.GetValue("Type").ToString());
            //Chat o = oobject.data.ToObject<Chat>();

            //Console.WriteLine(o.message);
        }
    }

    public class Root
    {
        public string Type { get; set; }
        public string sender { get; set; }
        public string target { get; set; }
        public Object data { get; set; }
    }

    public class Chat
    {
        public string message { get; set; }
    }

    public class HealthData
    {
        public int heartbeat { get; set; }
        public int rpm { get; set; }
        public double speed { get; set; }
        public int acc_watt { get; set; }
        public int tot_watt { get; set; }
    }

    public class Setting
    {
        public int res { get; set; }
    }

    public class Selection
    {
        public List<string> selection { get; set; }
    }
}
