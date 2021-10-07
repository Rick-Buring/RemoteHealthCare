using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationObjects
{
    public class Client : IDisposable
    {
        private const string certificateName = "testCertificaat";


        // __CR__ [PSMG] Streams e.d. worden nooit gedisposed. Implementeer IDisposable en implementeer daarin de disposing 
        private TcpClient client;
        private SslStream stream;
        public Client(TcpClient client)
        {
            this.client = client;
            this.stream = new SslStream(
                this.client.GetStream(),
                false,
                new RemoteCertificateValidationCallback(ValidateServerCertificate),
                null
            );
            stream.AuthenticateAsClient(certificateName);
        }

        public Client(TcpClient client, X509Certificate certificate)
        {
            this.client = client;
            this.stream = new SslStream(client.GetStream(), false);
            stream.AuthenticateAsServer(certificate, clientCertificateRequired: false, checkCertificateRevocation: true);
        }

        private static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;

            Debug.WriteLine("Certificate error: {0}", sslPolicyErrors);

            // Do not allow this client to communicate with unauthenticated servers.
            return false;
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
        public void Write(byte[] message)
        {
            try
            {
                stream.Write(WrapMessage(message));
                stream.Flush();
                // __CR__ [PSMG] Probeer niet alle exceptions te catchen
            } catch (Exception e)
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


        
		public void Dispose ()
		{
            this.stream.Close();
            this.stream.Dispose();
            this.client.Close();
            this.client.Dispose();
        }
	}
}
