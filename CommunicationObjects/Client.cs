using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace CommunicationObjects
{
    public class Client
    {
        private TcpClient tcpClient;
        private NetworkStream stream;

        public Client(TcpClient tcpClient)
        {
            this.tcpClient = tcpClient;
            this.stream = tcpClient.GetStream();
        }
        private static byte[] WrapMessage(byte[] message)
        {
            // Get the length prefix for the message
            byte[] lengthPrefix = BitConverter.GetBytes(message.Length);
            // Concatenate the length prefix and the message
            byte[] ret = new byte[lengthPrefix.Length + message.Length];
            lengthPrefix.CopyTo(ret, 0);
            message.CopyTo(ret, lengthPrefix.Length);
            return ret;
        }

        /// <summary>
        /// send messages to the client
        /// </summary>
        /// <param name="message">message to be sent to the client</param>
        public void send(byte[] message)
        {
            stream.Write(WrapMessage(message));
            stream.Flush();
        }

        public void terminate()
        {
            this.stream = null;
            this.tcpClient.Close();
            this.tcpClient.Dispose();
        }

        /// <summary>
        /// Reads incomming messages
        /// </summary>
        /// <returns>message in form of a string</returns>
        public string Read()
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
                //Console.WriteLine("ReadMessage: " + read);
            }

            return Encoding.ASCII.GetString(received);
        }
    }
}
