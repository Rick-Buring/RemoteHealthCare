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
            string dest = JObject.FromObject(JObject.Parse(tunnelOpen).GetValue("data")).GetValue("id").ToString();
            //client.Close();
            //client.Dispose();

           

       

            Skybox skybox = new Skybox();
            skybox.id = "scene/skybox/settime";
            skybox.data.time = 24;

            string message = WrapJsonMessage<Skybox>(dest, skybox);
            messageToSend = WrapMessage(Encoding.ASCII.GetBytes(message));
            client.GetStream().Write(messageToSend, 0, messageToSend.Length);
            client.GetStream().Flush();
            received = ReadMessage(client);
            string receivedMessage = Encoding.ASCII.GetString(received);
            Debug.WriteLine(receivedMessage);

            skybox.id = "scene/skybox/update";
            skybox.setType(Skybox.SkyboxType.STATIC);
            messageToSend = WrapMessage(Encoding.ASCII.GetBytes(WrapJsonMessage<Skybox>(dest, skybox)));
            client.GetStream().Write(messageToSend, 0, messageToSend.Length);
            client.GetStream().Flush();
            received = ReadMessage(client);
            receivedMessage = Encoding.ASCII.GetString(received);
            Debug.WriteLine(receivedMessage);

            Route r = new Route();
            r.addPosAndDirection(new float[3] { 4, 3, 2 }, new float[3] { 4, 3, 32 });
            r.addPosAndDirection(new float[3] { 4, 4, 2 }, new float[3] { 4, 34, 2 });
            r.addPosAndDirection(new float[3] { 4, 35, 2 }, new float[3] { 46, 3, 2 });
            r.addPosAndDirection(new float[3] { 4, 38, 2 }, new float[3] { 4, 3, 12 });
            messageToSend = WrapMessage(Encoding.ASCII.GetBytes(WrapJsonMessage<Route.RouteObject>(dest, r.addRoute())));
            client.GetStream().Write(messageToSend, 0, messageToSend.Length);
            client.GetStream().Flush();
            received = ReadMessage(client);
            receivedMessage = Encoding.ASCII.GetString(received);
            Debug.WriteLine(receivedMessage);
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
