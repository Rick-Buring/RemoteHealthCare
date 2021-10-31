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
using System.Threading;
using System.Windows;
using Vr_Project.RemoteHealthcare;
using VR_Project.Objects;
using VR_Project.Objects.Node;
using static VR_Project.ViewModels.ViewModel;
using VR_Project.ViewModels;

namespace VR_Project
{
    public class VrManager : IDisposable
    {

        private TcpClient client = new TcpClient("145.48.6.10", 6666);
        private Stream stream;
        private string dest;
        private string panelUuid;
        private string bikeUuid;
        private string cameraID;
        private float[] oldPos;
        private const string font = "segoeui";
        private const double fontSize = 50d;
        private bool running = false;
        private bool ready = false;

        private float requestedResistance = 50;
        public float resistanceMultiplier { get; } = 200f;

        private string lastMessage;

        public VrManager()
        {
            updater += Update;
            lastMessage = "";
        }

        public async Task<List<Data>> GetEngineData()
        {
            this.stream = this.client.GetStream();
            EngineRoot EngineRoot = new EngineRoot
            {
                id = "session/list"
            };
            string message = JsonConvert.SerializeObject(EngineRoot);
            byte[] messageArray = Encoding.ASCII.GetBytes(message);
            var messageToSend = WrapMessage(messageArray);
            await this.stream.WriteAsync(messageToSend, 0, messageToSend.Length);
            await this.stream.FlushAsync();
            byte[] array = new byte[4];


            await client.GetStream().ReadAsync(array, 0, 4);

            int size = BitConverter.ToInt32(array);
            if (size == 0)
                return null;

            byte[] received = new byte[size];


            int bytesRead = 0;
            while (bytesRead < size)
            {
                int read = await this.stream.ReadAsync(received, bytesRead, received.Length - bytesRead);
                bytesRead += read;
                Console.WriteLine("ReadMessage: " + read);
            }


            string test = Encoding.ASCII.GetString(received);
            EngineRoot = JsonConvert.DeserializeObject<EngineRoot>(test);

            List<Data> OnlineEngines = new List<Data>();
            if (EngineRoot != null)
            {
                foreach (Data d in EngineRoot.data)
                    OnlineEngines.Add(d);

                return OnlineEngines;
            } return null;
        }
        public async Task ConnectToTunnel(string tunnelID)
        {

            string tunnel = @"{""id"" : ""tunnel/create"", ""data"" :{""session"" : """ + tunnelID + @"""}}";
            byte[] messageToSend = WrapMessage(Encoding.ASCII.GetBytes(tunnel));
            await this.stream.WriteAsync(messageToSend, 0, messageToSend.Length);
            await this.stream.FlushAsync();

            byte[] received = await ReadMessage(client);
            string tunnelOpen = Encoding.ASCII.GetString(received);
            Debug.WriteLine(tunnelOpen);

            this.dest = JObject.FromObject(JObject.Parse(tunnelOpen).GetValue("data")).GetValue("id").ToString();

            await ResetScene();

            await ChangeSkyBoxTime(15);

            await DeleteGroundPlane();

            await AddTerrain();
            this.bikeUuid = await MakeBikeObject();
            await MakePanel(this.bikeUuid);
            await MakeAndFollowRoute(this.bikeUuid);
            this.cameraID = await GetCamera();
            await StickCameraToPlayer();
            this.oldPos = await getPosition();
            this.ready = true;
        }

        private async Task ResetScene()
        {
            string request = @"{ ""id"" : ""scene/reset"" }";

            JObject sceneResponse = await SendMessageResponseToJsonArray(client, WrapJsonMessage(dest, request));
            Debug.WriteLine(sceneResponse.ToString());
        }

        public async Task<string> MakeBikeObject()
        {

            ObjectNode bikeNode = new ObjectNode("scene/node/add", "bike", @"data\NetworkEngine\models\cars\generic\Carpet.obj", new int[] { 15, 15, 15 });

            JObject BikeResponse = await SendMessageResponseToJsonArray(client, WrapJsonMessage<ObjectNode>(this.dest, bikeNode));


            return BikeResponse.Value<JObject>("data").Value<JObject>("data").Value<JObject>("data").Value<string>("uuid");
        }
        public async Task<string> GetScene()
        {
            string request = @"{ ""id"" : ""scene/get"" }";
            JObject sceneResponse = await SendMessageResponseToJsonArray(client, WrapJsonMessage(dest, request));
            Debug.WriteLine(sceneResponse.ToString());
            return sceneResponse.ToString();
        }
        public async Task<string> GetCamera()
        {
            string request = @"{ ""id"" : ""scene/node/find"", ""data"": {""name"" : ""Camera""}}";
            JObject cameraResponse = await SendMessageResponseToJsonArray(client, WrapJsonMessage(dest, request));
            Debug.WriteLine(cameraResponse.ToString());
            return GetCameraID(cameraResponse);
        }
        public void RequestResistance(float resistance)
        {
            this.requestedResistance = resistance;
        }
        public async void Update(Ergometer ergometer, HeartBeatMonitor heartBeatMonitor)
        {
            if (this.ready && !this.running)
            {
                this.running = true;
                try
                {
                    if (this.stream.CanRead && this.stream.CanWrite)
                    {
                        await UpdateSpeed(ergometer.GetErgometerData().Cadence / 13);
                        if (this.stream.CanRead && this.stream.CanWrite)
                            await WriteToPanel(ergometer.GetErgometerData(), heartBeatMonitor.GetHeartBeat());
                    }
                    
                    float r = await getHeightDependantResistance();

                    ViewModel.resistanceUpdater(CalculateResistance(r));
                } catch (IOException e)
                {
                    Debug.WriteLine(e.StackTrace);
                }

                this.running = false;
            }
        }

        public void SetChatMessage(string message)
		{
            this.lastMessage = message;
		}
        private float CalculateResistance(float heightResistance)
        {
            return this.requestedResistance * heightResistance;
        }
        public async Task UpdateSpeed(double speed)
        {
            
            Route r = new Route();
            Route.RouteObject rObject = r.UpdateFollowSpeed(this.bikeUuid, (float)speed);
            await SendMessage(this.client, WrapJsonMessage<Route.RouteObject>(this.dest, rObject));
        }
        public async Task StickCameraToPlayer()
        {
            UpdateNode node = new UpdateNode("scene/node/update", this.cameraID, this.bikeUuid, new double[] { 90, 90, 0 });
            JObject updateResponse = await SendMessageResponseToJsonArray(client, WrapJsonMessage<UpdateNode>(dest, node));
            Debug.WriteLine(updateResponse.ToString());
        }
        private string GetCameraID(JObject cameraObject)
        {
            return cameraObject.Value<JObject>("data").Value<JObject>("data").Value<JArray>("data").ElementAt(0).Value<string>("uuid");
        }
        public async Task MakePanel(string parentID)
        {
            PanelNode panelNode = new PanelNode("scene/node/add", "dataPanel", parentID);

            JObject panelResponse = await SendMessageResponseToJsonArray(client, WrapJsonMessage<PanelNode>(this.dest, panelNode));


            this.panelUuid = panelResponse.Value<JObject>("data").Value<JObject>("data").Value<JObject>("data").Value<string>("uuid");

            Panel panel = new Panel();
            panel.setclearColor(this.panelUuid, new int[] { 1, 1, 1, 1 });
            await SendMessage(client, WrapJsonMessage<Panel>(this.dest, panel));
        }
        public async Task WriteToPanel(ErgometerData ergometerData, int heartBeat)
        {
            if (this.panelUuid != null && ergometerData != null)
            {
                Panel panel = new Panel();
                panel.Clear(this.panelUuid);
                await SendMessage(client, WrapJsonMessage<Panel>(this.dest, panel));
                panel.drawText(this.panelUuid, "RPM : " + ergometerData.Cadence, new double[] { 10d, 40d }, fontSize, new int[] { 0, 0, 0, 1 }, font);
                await SendMessage(client, WrapJsonMessage<Panel>(this.dest, panel));
                panel.drawText(this.panelUuid, "BPM : " + heartBeat, new double[] { 10d, 80d }, fontSize, new int[] { 0, 0, 0, 1 }, font);
                await SendMessage(client, WrapJsonMessage<Panel>(this.dest, panel));
                panel.drawText(this.panelUuid, "Wattage : " + ergometerData.InstantaneousPower, new double[] { 10d, 120d }, fontSize, new int[] { 0, 0, 0, 1 }, font);
                await SendMessage(client, WrapJsonMessage<Panel>(this.dest, panel));
                panel.drawText(this.panelUuid, "Speed (km/h) : " + ergometerData.InstantaneousSpeed, new double[] { 10d, 160d }, fontSize, new int[] { 0, 0, 0, 1 }, font);
                await SendMessage(client, WrapJsonMessage<Panel>(this.dest, panel));
                panel.drawText(this.panelUuid, "Distance Traveled: (m) : " + ergometerData.DistanceTraveled, new double[] { 10d, 200d }, fontSize, new int[] { 0, 0, 0, 1 }, font);
                await SendMessage(client, WrapJsonMessage<Panel>(this.dest, panel));
                string sec = ergometerData.ElapsedTime % 60 + "";
                if ((ergometerData.ElapsedTime % 60) < 10) sec = 0 + sec;
                string min = Math.Floor((decimal)ergometerData.ElapsedTime / 60) + "";
                string time = string.Format($"Elapsed Time: {min}:{sec}");
                panel.drawText(this.panelUuid, time, new double[] { 10d, 240d }, fontSize, new int[] { 0, 0, 0, 1 }, font);
                await SendMessage(client, WrapJsonMessage<Panel>(this.dest, panel));
                panel.drawText(this.panelUuid, this.lastMessage, new double[] { 10d, 320d }, fontSize, new int[] { 0, 0, 0, 1 }, font);
                await SendMessage(client, WrapJsonMessage<Panel>(this.dest, panel));
                panel.Swap(this.panelUuid);
                await SendMessage(client, WrapJsonMessage<Panel>(this.dest, panel));





            }

        }
        public async Task MakeAndFollowRoute(string nodeID)
        {
            Route r = await Route.getRouteWithHeight(this);
            JObject routeResponse = await SendMessageResponseToJsonArray(this.client, WrapJsonMessage<Route.RouteObject>(this.dest, r.addRoute(true)));
            string routeID = routeResponse.Value<JObject>("data").Value<JObject>("data").Value<JObject>("data").Value<string>("uuid");
            Road road = new Road("scene/road/add", routeID);

            await SendMessage(client, WrapJsonMessage<Road>(dest, road));

            await SendMessage(client, WrapJsonMessage<Route.RouteObject>(dest, r.followRoute(routeID, nodeID, 2)));
        }
        public async Task ChangeSkyBoxTime(int time)
        {
            Skybox skybox = new Skybox
            {
                id = "scene/skybox/settime",
                data =
                {
                    time = time
                }
            };

            await SendMessage(client, WrapJsonMessage<Skybox>(this.dest, skybox));

            skybox.id = "scene/skybox/update";

            await SendMessage(client, WrapJsonMessage<Skybox>(this.dest, skybox));
        }
        public async Task DeleteGroundPlane()
        {
            Node findNode = new Node("scene/node/find")
            {
                data =
                {
                    name = "GroundPlane"
                }
            };

            JObject jObject = await SendMessageResponseToJsonArray(this.client, WrapJsonMessage<Node>(this.dest, findNode));

            string uuid = jObject.Value<JObject>("data").Value<JObject>("data")?.Value<JArray>("data")[0].Value<string>("uuid");

            Node deleteNode = new Node("scene/node/delete")
            {
                data =
                {
                    id = uuid
                }
            };

            await SendMessage(client, WrapJsonMessage<Node>(this.dest, deleteNode));
        }
        public async Task Add3dObjects()
        {
            ObjectNode ObjectNode1 = new ObjectNode("scene/node/add", "object1", @"data\NetworkEngine\models\trees\fantasy\tree1.obj", new int[3] { 1, 2, 1 });

            await SendMessage(client, WrapJsonMessage<ObjectNode>(this.dest, ObjectNode1));

            ObjectNode ObjectNode2 = new ObjectNode("scene/node/add", "object1", @"data\NetworkEngine\models\trees\fantasy\tree1.obj", new int[3] { 0, 3, 1 });

            await SendMessage(client, WrapJsonMessage<ObjectNode>(this.dest, ObjectNode2));

            ObjectNode ObjectNode3 = new ObjectNode("scene/node/add", "object1", @"data\NetworkEngine\models\trees\fantasy\tree1.obj", new int[3] { 0, 0, 0 });

            await SendMessage(client, WrapJsonMessage<ObjectNode>(this.dest, ObjectNode3));
        }
        public async Task AddTerrain()
        {

            Terrain terrain = new Terrain("scene/terrain/add", new int[] { 256, 256 }, Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + "/HeightmapSmol.bmp");

            await SendMessage(client, WrapJsonMessage<Terrain>(this.dest, terrain));

            TerrainNode node = new TerrainNode("scene/node/add", "terrainNode", true);

            JObject response = await SendMessageResponseToJsonArray(client, WrapJsonMessage<TerrainNode>(this.dest, node));

            string uuid = response.Value<JObject>("data").Value<JObject>("data").Value<JObject>("data").Value<string>("uuid");

            AddLayerNode layerNode = new AddLayerNode("scene/node/addlayer", uuid, @"data\NetworkEngine\textures\terrain\adesert_mntn2_d.jpg");
            response = await SendMessageResponseToJsonArray(client, WrapJsonMessage<AddLayerNode>(this.dest, layerNode));
        }
        public async Task<float> getHeightDependantResistance()
        {
            float[] position = await getPosition();
            float distance = (float)Math.Sqrt(Math.Pow(position[0] - oldPos[0], 2) + Math.Pow(position[2] - oldPos[2], 2));
            float Heightdifference = position[1] - oldPos[1];
            float angle = 0;
            if (distance != 0 || Heightdifference != 0)
            {
                angle = (float)((Math.PI / 180) * Math.Atan(Heightdifference / distance));
            }
            Debug.WriteLine($"calculated height dif: {Heightdifference} \nwith the distance of: {distance} \nmakes the angle of: {angle}");
            float resistance = (float)Math.Round(Math.Clamp(1 + angle * this.resistanceMultiplier, 0, 2), 2);
            Debug.WriteLine($"resistance: {resistance}");
            this.oldPos = position;
            return resistance;
        }
        public async Task<float[]> getPosition()
        {
            Node findNode = new Node("scene/node/find");
            findNode.data.name = "bike";
            JObject jObject = await SendMessageResponseToJsonArray(this.client, WrapJsonMessage<Node>(this.dest, findNode));
            JArray jPossition = jObject.Value<JObject>("data").Value<JObject>("data").Value<JArray>("data").ElementAt(0).Value<JArray>("components").ElementAt(0).Value<JArray>("position");
            return new float[3] { (float)jPossition[0], (float)jPossition[1], (float)jPossition[2] };
        }
        public async Task<float> getTerrainHeight(float[] position)
        {
            Terrain t = new Terrain("scene/terrain/getheight", position);
            JObject terrainResponce = await SendMessageResponseToJsonArray(client, WrapJsonMessage<Terrain>(dest, t));
            return terrainResponce.Value<JObject>("data").Value<JObject>("data").Value<JObject>("data").Value<float>("height");
        }
        public static string WrapJsonMessage<T>(string dest, T t)
        {
            string objectToJson = JsonConvert.SerializeObject(t);

            string message = @"{""id"" : ""tunnel/send"", ""data"" : " + @"{""dest"" : """ + dest + @""", ""data"" : " + objectToJson + "}}";

            return message;
        }
        public static string WrapJsonMessage(string dest, string request)
        {
            string message = @"{""id"" : ""tunnel/send"", ""data"" : " + @"{""dest"" : """ + dest + @""", ""data"" : " + request + "}}";
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
        public async Task SendMessage(TcpClient client, string message)
        {
            byte[] messageToSend = WrapMessage(Encoding.ASCII.GetBytes(message));
            await this.stream.WriteAsync(messageToSend, 0, messageToSend.Length);
            await this.stream.FlushAsync();

            string received = Encoding.ASCII.GetString(await ReadMessage(client));
        }
        public async Task<JObject> SendMessageResponseToJsonArray(TcpClient client, string message)
        {

            byte[] messageToSend = WrapMessage(Encoding.ASCII.GetBytes(message));
            await this.stream.WriteAsync(messageToSend, 0, messageToSend.Length);
            await this.stream.FlushAsync();
            return (JObject)JsonConvert.DeserializeObject(Encoding.ASCII.GetString(await ReadMessage(client)));
        }
        public async Task<byte[]> ReadMessage(TcpClient client)
        {
            byte[] array = new byte[4];
            await stream.ReadAsync(array, 0, 4);

            int size = BitConverter.ToInt32(array);
            byte[] received = new byte[size];


            int bytesRead = 0;
            while (bytesRead < size)
            {
                int read = await this.stream.ReadAsync(received, bytesRead, received.Length - bytesRead);
                bytesRead += read;
            }


            return received;

        }
        public void CloseConnection()
        {
            this.client.Close();
            this.client.Dispose();
        }

        public void Dispose()
        {
            this.stream.Close();
            this.stream.Dispose();
            this.client.Close();
            this.client.Dispose();
        }
    }
}
