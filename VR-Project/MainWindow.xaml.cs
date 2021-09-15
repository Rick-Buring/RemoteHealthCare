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
          
            Root t = new Root();
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
            Root root = JsonConvert.DeserializeObject<Root>(test);

            //root.data.ElementAt(0).id; is de session id.
            string id = root.data.ElementAt(0).id;

            string tunnel = @"{""id"" : ""tunnel/create"", ""data""{""""session"" : " + id + @""",""key"" : """"}}";
            

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
    }
}
