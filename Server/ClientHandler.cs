using System;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    public class ClientHandler
    {
        private TcpClient tcpClient;
        private NetworkStream stream;

        public ClientHandler(TcpClient tcpClient)
        {
            this.tcpClient = tcpClient;
            this.stream = this.tcpClient.GetStream();
        }

        private void Read()
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
        }

        private void Parse(string toParse)
        {
            
        }
    }
}
