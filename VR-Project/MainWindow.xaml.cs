using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
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
        public MainWindow()
        {
            InitializeComponent();
            TcpClient client = new TcpClient("145.48.6.10", 6666);

            StreamWriter writer = new StreamWriter(client.GetStream());
            StreamReader reader = new StreamReader(client.GetStream());
          
            Test t = new Test();
            t.id = "session/list";
            //string message = @"{""id"" : ""session/list""}";
            string message = JsonConvert.SerializeObject(t);
            Debug.WriteLine(message);
            byte[] messageArray = Encoding.ASCII.GetBytes(message);
            var messageToSend = WrapMessage(messageArray);
            client.GetStream().Write(messageToSend, 0, messageToSend.Length);
            //writer.Write(, 0);
            writer.Flush();
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
            ////test = @"{""id"":""session/list"",""data"":[{""id"":""2992f621-963b-46e8-9bae-fef83f4a947c"",""beginTime"":""2021-09-15T11:54:31.1809543+00:00"",""lastPing"":""2021-09-15T11:54:31.2034461+00:00"",""fps"":[{""time"":4.7212173,""fps"":60.0},{""time"":14.717631899999999,""fps"":1618.8},{""time"":24.7210616,""fps"":2237.8},{""time"":34.7254261,""fps"":2289.3},{""time"":44.7307312,""fps"":2313.3},{""time"":54.7310279,""fps"":2299.0},{""time"":64.7371666,""fps"":2298.5},{""time"":74.7390066,""fps"":2326.6},{""time"":84.742751,""fps"":2327.3},{""time"":94.744913,""fps"":2328.7},{""time"":104.74929619999999,""fps"":2327.9},{""time"":114.7530228,""fps"":2286.3},{""time"":124.7547433,""fps"":2298.0},{""time"":134.75900439999998,""fps"":2322.1},{""time"":144.7695984,""fps"":2316.1},{""time"":154.7667746,""fps"":2323.1},{""time"":164.77068269999998,""fps"":2321.8},{""time"":174.77244869999998,""fps"":2298.4},{""time"":184.7764681,""fps"":2281.0},{""time"":194.78036079999998,""fps"":2291.9},{""time"":204.7844184,""fps"":2332.6},{""time"":214.7868111,""fps"":2318.5},{""time"":224.7904112,""fps"":2302.5},{""time"":234.794297,""fps"":2308.6},{""time"":244.8022718,""fps"":1695.3},{""time"":254.8003128,""fps"":1719.8},{""time"":264.80424089999997,""fps"":1636.8},{""time"":274.8082774,""fps"":1631.0},{""time"":284.8249982,""fps"":1971.4},{""time"":294.8166043,""fps"":2241.1},{""time"":304.81880229999996,""fps"":2268.3},{""time"":314.8220862,""fps"":2302.3},{""time"":324.83413659999997,""fps"":1873.8},{""time"":334.83038039999997,""fps"":2148.5}]}]}";
            //var jsonData = JsonConvert.DeserializeObject(test);
            ////JObject rootObject = JObject.Parse(test);
            //JObject root = JObject.Parse(jsonData.ToString());
            //string id = root.GetValue("id").ToString();
            

        }

        public static byte[] WrapMessage(byte[] message)
        {
            // Get the length prefix for the message
            byte [] lengthPrefix = BitConverter.GetBytes(message.Length);
            // Concatenate the length prefix and the message
            byte [] ret = new byte [lengthPrefix.Length + message.Length];
            lengthPrefix.CopyTo(ret, 0);
            message.CopyTo(ret, lengthPrefix.Length);
            return ret;
        }
        class Test
        {
            public string ?id { get; set; }

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

        public class Datum
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
            public List<Datum> ?data { get; set; }
        }
    }
}
