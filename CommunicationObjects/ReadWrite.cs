using CommunicationObjects.DataObjects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationObjects
{
    public class ReadWrite : IDisposable
    {
        public const string certificateName = "testCertificaat";
        private SslStream stream;

        public ReadWrite(SslStream stream)
        {
            this.stream = stream;
        }

        public static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;

            Debug.WriteLine("Certificate error: {0}", sslPolicyErrors);

            // Do not allow this client to communicate with unauthenticated servers.
            return false;
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
           
        /// <summary>
        /// send messages to the client
        /// </summary>
        /// <param name="message">message to be sent to the client</param>
        public void Write(byte[] message)
        {
            try
            {
                stream.Write(WrapMessage(message));
                stream.Flush();
                // __CR__ [PSMG] Probeer niet alle exceptions te catchen
                //Fixed, Exception => IOException
            } catch (IOException e)
            {
                Debug.WriteLine(e.Message);
            } catch (NotSupportedException e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// Reads incomming messages
        /// </summary>
        /// <returns>message in form of a string</returns>
        public async Task<string> Read()
        {

            byte[] length = new byte[4];
            this.stream.Read(length, 0, 4);

            int size = BitConverter.ToInt32(length);

            byte[] received = new byte[size];

            int bytesRead = 0;
            while (bytesRead < size)
            {
                int read = await this.stream.ReadAsync(received, bytesRead, received.Length - bytesRead);
                bytesRead += read;
                //Console.WriteLine("ReadMessage: " + read);
            }

            return Encoding.ASCII.GetString(received);
        }

        public void Dispose()
        {
            this.stream.Close();
            this.stream.Dispose();
        }
    }
}
