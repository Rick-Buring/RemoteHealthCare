using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VR_Project
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TcpClient client = new TcpClient("145.48.6.10", 6666);
        private string dest;

        public MainWindow()
        {
            Root t = new Root();
            t.id = "session/list";
            //string message = @"{""id"" : ""session/list""}";
            string message = JsonConvert.SerializeObject(t);
            //Debug.WriteLine(message);
            byte[] messageArray = Encoding.ASCII.GetBytes(message);
            var messageToSend = WrapMessage(messageArray);
            client.GetStream().Write(messageToSend, 0, messageToSend.Length);
            client.GetStream().Flush();
            //writer.Write(, 0);
            //writer.Flush();
            //Console.WriteLine(reader.ReadToEnd());
            byte[] array = new byte[4];


            client.GetStream().Read(array, 0, 4);

            //int line = reader.Read();
            int size = BitConverter.ToInt32(array);
            byte[] received = new byte[size];


            int bytesRead = 0;
            while (bytesRead < size)
            {
                int read = client.GetStream().Read(received, bytesRead, received.Length - bytesRead);
                bytesRead += read;
                Console.WriteLine("ReadMessage: " + read);
            }

            //client.GetStream().Read(received, 0, size);


            string test = Encoding.ASCII.GetString(received);
            //Debug.WriteLine(test);
            root = JsonConvert.DeserializeObject<Root>(test);

            ob = new ObservableCollection<Data>();
            InitializeComponent();
            
            DataContext = this;
            foreach (Data d in root.data)
                ob.Add(d);

           

        }
        private Root root;
        public ObservableCollection<Data> ob { get; set; }

        private string tunnelID;
        private Data _selectedMilight;

        public Data SelectedMilight
        {
            get { return _selectedMilight; }
            set
            {
                _selectedMilight = value;
            }
        }


        private void connectToTunnel()
        {
            string tunnel = @"{""id"" : ""tunnel/create"", ""data"" :{""session"" : """ + tunnelID + @"""}}";
            byte[] messageToSend = WrapMessage(Encoding.ASCII.GetBytes(tunnel));
            client.GetStream().Write(messageToSend, 0, messageToSend.Length);
            client.GetStream().Flush();

            byte[] received = ReadMessage(client);
            string tunnelOpen = Encoding.ASCII.GetString(received);
            Debug.WriteLine(tunnelOpen);

            var destVar = JsonConvert.DeserializeObject(tunnelOpen);
            this.dest = JObject.FromObject(JObject.Parse(tunnelOpen).GetValue("data")).GetValue("id").ToString();


            Skybox skybox = new Skybox();
            skybox.id = "scene/skybox/settime";
            skybox.data.time = 24;

            SendMessage(client, WrapJsonMessage<Skybox>(this.dest, skybox));

            skybox.id = "scene/skybox/update";
 //           skybox.setType(Skybox.SkyboxType.STATIC);
      
            SendMessage(client, WrapJsonMessage<Skybox>(this.dest, skybox));

            deleteGroundPlane();

            Terrain terrain = new Terrain("scene/terrain/add", new int[] { 256, 256 }, Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + "/Heightmap.txt");

            SendMessage(client, WrapJsonMessage<Terrain>(this.dest, terrain));

            TerrainNode node = new TerrainNode("scene/node/add", "terrainNode", true);

            SendMessage(client, WrapJsonMessage<TerrainNode>(this.dest, node));


  

            client.Close();
            client.Dispose();
        }

        public void deleteGroundPlane()
        {
            Node findNode = new Node("scene/node/find");
            findNode.data.name = "GroundPlane";

            JObject jObject;
            SendMessageJsonArray(this.client, WrapJsonMessage<Node>(this.dest, findNode), out jObject);

 


            string uuid = jObject.Value<JObject>("data").Value<JObject>("data").Value<JArray>("data")[0].Value<string>("uuid");

            Node deleteNode = new Node("scene/node/delete");
            deleteNode.data.id = uuid;

            SendMessage(client, WrapJsonMessage<Node>(this.dest, deleteNode));
        }

        public static string WrapJsonMessage<T> (string dest, T t)
        {
            string objectToJson = JsonConvert.SerializeObject(t);

            string message = @"{""id"" : ""tunnel/send"", ""data"" : " + @"{""dest"" : """ + dest + @""", ""data"" : " + objectToJson + "}}";

            return message;
        }

        public static byte[] WrapMessage(byte[] message)
        {
            // Get the length prefix for the message
            byte[] lengthPrefix = BitConverter.GetBytes(message.Length);
            // Concatenate the length prefix and the message
            byte[] ret = new byte[lengthPrefix.Length + message.Length];
            lengthPrefix.CopyTo(ret, 0);
            message.CopyTo(ret, lengthPrefix.Length);
            return ret;
        }
        


        //public static string getCorrectID(Root root)
        //{
        //    foreach (Data d in root.data)
        //    {
        //        if (d.clientinfo.host.Equals(Environment.MachineName))
        //        {
        //            return d.id;
        //        }
        //    }
        //    Debug.WriteLine("Could not find desktop");
        //    return "0XFF";
        //}

        public static void SendMessage(TcpClient client, string message)
        {
            Debug.WriteLine(message);
            byte[] messageToSend = WrapMessage(Encoding.ASCII.GetBytes(message));
            client.GetStream().Write(messageToSend, 0, messageToSend.Length);
            client.GetStream().Flush();
            Debug.WriteLine(Encoding.ASCII.GetString(ReadMessage(client)));
        }

        public static void SendMessageJsonArray(TcpClient client, string message, out JObject jObject)
        {
            Debug.WriteLine(message);
            byte[] messageToSend = WrapMessage(Encoding.ASCII.GetBytes(message));
            client.GetStream().Write(messageToSend, 0, messageToSend.Length);
            client.GetStream().Flush();
            jObject = (JObject)JsonConvert.DeserializeObject(Encoding.ASCII.GetString(ReadMessage(client)));
        }

        public static byte[] ReadMessage(TcpClient client)
        {
            //Console.WriteLine(reader.ReadToEnd());
            byte[] array = new byte[4];

            client.GetStream().Read(array, 0, 4);

            //int line = reader.Read();
            int size = BitConverter.ToInt32(array);
            byte[] received = new byte[size];


            int bytesRead = 0;
            while (bytesRead < size)
            {
                int read = client.GetStream().Read(received, bytesRead, received.Length - bytesRead);
                bytesRead += read;
                Console.WriteLine("ReadMessage: " + read);
            }

            return received;

        }


        public class Fp
        {
            public double ?time { get; set; }
            public double ?fps { get; set; }
        }

        public class Clientinfo
        {
            public string ?host { get; set; }
            public string ?user { get; set; }
            public string ?file { get; set; }
            public string ?renderer { get; set; }
        }
        
        public class Data
        {
            public string ?id { get; set; }
            public DateTime ?beginTime { get; set; }
            public DateTime ?lastPing { get; set; }
            public List<Fp> ?fps { get; set; }
            public List<string> ?features { get; set; }
            public Clientinfo ?clientinfo { get; set; }
        }

        public class Root
        {
            public string ?id { get; set; }
            public List<Data> ?data { get; set; }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedMilight == null)
                return;
            this.tunnelID = SelectedMilight.id;
            connectToTunnel();
            Debug.WriteLine(SelectedMilight.id);
        }
    }
}
