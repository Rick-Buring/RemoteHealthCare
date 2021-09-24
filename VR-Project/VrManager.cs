using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using Vr_Project.RemoteHealthcare;
using VR_Project.Objects;

namespace VR_Project
{
    public class VrManager
    {

        private TcpClient client = new TcpClient("145.48.6.10", 6666);
        private string dest;
        private string panelUuid;
        private string bikeUuid;

        public VrManager(EngineCallback listener)
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

            ObservableCollection<Data> ob = new ObservableCollection<Data>();

            foreach (Data d in root.data)
                ob.Add(d);

            listener.notify(ob);

        }

 

        private Root root;

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


        public void connectToTunnel(string tunnelID)
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

            changeSkyBoxTime(15);

            deleteGroundPlane();

            addTerrain();
            this.bikeUuid = makeBikeObject();
            MakePanel(this.bikeUuid);
            MakeAndFollowRoute(this.bikeUuid);
        }

        public string makeBikeObject()
        {

            ObjectNode bikeNode = new ObjectNode("scene/node/add", "bike", @"data\NetworkEngine\models\cars\generic\Carpet.obj", new int[3] { 15, 15, 15 });

            JObject BikeResponse;

            SendMessageResponseToJsonArray(client, WrapJsonMessage<ObjectNode>(this.dest, bikeNode), out BikeResponse);

            return BikeResponse.Value<JObject>("data").Value<JObject>("data").Value<JObject>("data").Value<string>("uuid");
        }

        public void MakePanel (string parentID)
        {
            PanelNode panelNode = new PanelNode("scene/node/add", "dataPanel", parentID);

            JObject panelResponse;

            SendMessageResponseToJsonArray(client, WrapJsonMessage<PanelNode>(this.dest, panelNode), out panelResponse);

            this.panelUuid = panelResponse.Value<JObject>("data").Value<JObject>("data").Value<JObject>("data").Value<string>("uuid");

            Panel panel = new Panel();
            panel.setclearColor(this.panelUuid, new int[] { 1, 1, 1, 1 });
            SendMessage(client, WrapJsonMessage<Panel>(this.dest, panel));
        }

        public void WriteToPanel (int rpm)
        {
            if (this.panelUuid != null)
            {
                Panel panel = new Panel();

                panel.Clear(this.panelUuid);
                SendMessage(client, WrapJsonMessage<Panel>(this.dest, panel));

                panel.drawText(this.panelUuid, "RPM : " + rpm, new double[] { 10d, 20d }, 30d, new int[] { 0, 0, 0, 1 }, "segoeui");
                JObject response;
                SendMessageResponseToJsonArray(client, WrapJsonMessage<Panel>(this.dest, panel), out response);
                Debug.WriteLine(response.ToString());

                panel.Swap(this.panelUuid);
                SendMessage(client, WrapJsonMessage<Panel>(this.dest, panel));
                //Debug.WriteLine(writeResponse.ToString());
            }

        }

        public void MakeAndFollowRoute(string nodeID)
        {
            JObject routeResponse;
            Route r = Route.getRoute();
            SendMessageResponseToJsonArray(this.client, WrapJsonMessage<Route.RouteObject>(this.dest, r.addRoute(true)), out routeResponse);
            string routeID = routeResponse.Value<JObject>("data").Value<JObject>("data").Value<JObject>("data").Value<string>("uuid");
            Road road = new Road("scene/road/add", routeID);

            SendMessage(client, WrapJsonMessage<Road>(dest, road));

            SendMessage(client, WrapJsonMessage<Route.RouteObject>(dest, r.followRoute(routeID, nodeID, 2)));
        }

        public void changeSkyBoxTime(int time)
        {
            Skybox skybox = new Skybox();
            skybox.id = "scene/skybox/settime";
            skybox.data.time = time;

            SendMessage(client, WrapJsonMessage<Skybox>(this.dest, skybox));

            skybox.id = "scene/skybox/update";

            SendMessage(client, WrapJsonMessage<Skybox>(this.dest, skybox));
        }

        public void deleteGroundPlane()
        {
            Node findNode = new Node("scene/node/find");
            findNode.data.name = "GroundPlane";

            JObject jObject;
            SendMessageResponseToJsonArray(this.client, WrapJsonMessage<Node>(this.dest, findNode), out jObject);


            string uuid = jObject.Value<JObject>("data").Value<JObject>("data").Value<JArray>("data")[0].Value<string>("uuid");

            Node deleteNode = new Node("scene/node/delete");
            deleteNode.data.id = uuid;

            SendMessage(client, WrapJsonMessage<Node>(this.dest, deleteNode));
        }

        public void add3dObjects()
        {
            ObjectNode ObjectNode1 = new ObjectNode("scene/node/add", "object1", @"data\NetworkEngine\models\trees\fantasy\tree1.obj", new int[3] { 1, 2, 1 });

            SendMessage(client, WrapJsonMessage<ObjectNode>(this.dest, ObjectNode1));

            ObjectNode ObjectNode2 = new ObjectNode("scene/node/add", "object1", @"data\NetworkEngine\models\trees\fantasy\tree1.obj", new int[3] { 0, 3, 1 });

            SendMessage(client, WrapJsonMessage<ObjectNode>(this.dest, ObjectNode2));

            ObjectNode ObjectNode3 = new ObjectNode("scene/node/add", "object1", @"data\NetworkEngine\models\trees\fantasy\tree1.obj", new int[3] { 0, 0, 0 });

            SendMessage(client, WrapJsonMessage<ObjectNode>(this.dest, ObjectNode3));
        }

        public void addTerrain()
        {

            Terrain terrain = new Terrain("scene/terrain/add", new int[] { 256, 256 }, Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + "/HeightmapSmol.bmp");

            SendMessage(client, WrapJsonMessage<Terrain>(this.dest, terrain));

            TerrainNode node = new TerrainNode("scene/node/add", "terrainNode", true);

            JObject response;

            SendMessageResponseToJsonArray(client, WrapJsonMessage<TerrainNode>(this.dest, node), out response);

            string uuid = response.Value<JObject>("data").Value<JObject>("data").Value<JObject>("data").Value<string>("uuid");

            AddLayerNode layerNode = new AddLayerNode("scene/node/addlayer", uuid, @"data\NetworkEngine\textures\terrain\adesert_mntn2_d.jpg");

            SendMessage(client, WrapJsonMessage<AddLayerNode>(this.dest, layerNode));
        }

        public static string WrapJsonMessage<T>(string dest, T t)
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

        public static void SendMessageResponseToJsonArray(TcpClient client, string message, out JObject jObject)
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

        public void UpdatePanel (Ergometer ergometer)
        {

        }

        public void CloseConnection ()
        {
            this.client.Close();
            this.client.Dispose();
        }


        public class Fp
        {
            public double? time { get; set; }
            public double? fps { get; set; }
        }

        public class Clientinfo
        {
            public string? host { get; set; }
            public string? user { get; set; }
            public string? file { get; set; }
            public string? renderer { get; set; }
        }

        public class Data
        {
            public string? id { get; set; }
            public DateTime? beginTime { get; set; }
            public DateTime? lastPing { get; set; }
            public List<Fp>? fps { get; set; }
            public List<string>? features { get; set; }
            public Clientinfo? clientinfo { get; set; }
        }

        public class Root
        {
            public string? id { get; set; }
            public List<Data>? data { get; set; }
        }

        //private void button_Click(object sender, RoutedEventArgs e)
        //{
        //    if (SelectedMilight == null)
        //        return;
        //    this.tunnelID = SelectedMilight.id;
            
        //}

    }
}
