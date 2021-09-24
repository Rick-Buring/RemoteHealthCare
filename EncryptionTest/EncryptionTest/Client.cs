using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;

namespace EncryptionTest
{
    class Client
    {
        private static Hashtable certificateErrors = new Hashtable();
        private static SslStream stream;
        private static TcpClient client;

        // The following method is invoked by the RemoteCertificateValidationDelegate.
        public static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;

            Debug.WriteLine("Certificate error: {0}", sslPolicyErrors);

            // Do not allow this client to communicate with unauthenticated servers.
            return false;
        }
        public static void RunClient(string machineName, string certificateName)
        {
            // Create a TCP/IP client socket.
            // machineName is the host running the server application.
            client = new TcpClient(machineName, 8080);
            Console.WriteLine("Client connected.");
            // Create an SSL stream that will close the client's stream.
            stream = new SslStream(
                client.GetStream(),
                false,
                new RemoteCertificateValidationCallback(ValidateServerCertificate),
                null
                );
            // The server name must match the name on the server certificate.
            try
            {
                stream.AuthenticateAsClient(certificateName);
            }
            catch (AuthenticationException e)
            {
                Debug.WriteLine("Exception: {0}", e.Message);
                if (e.InnerException != null)
                {
                    Console.WriteLine("Inner exception: {0}", e.InnerException.Message);
                }
                Debug.WriteLine("Authentication failed - closing the connection.");
                client.Close();
                return;
            }

            new Thread(ReadMessages).Start();

            while (true)
            {
                Write(Encoding.ASCII.GetBytes(Console.ReadLine()));
            }

        }

        public static void Write(byte[] message)
        {
            try
            {
                stream.Write(WrapMessage(message));
                stream.Flush();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
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
        public static void ReadMessages()
        {
            while(true)
            {
                string received = Read(stream);
                Console.WriteLine(received);
                if (received == "BYE")
                    close();
            }
        }

        public static string Read(SslStream stream)
        {
            byte[] length = new byte[4];
            stream.Read(length, 0, 4);

            int size = BitConverter.ToInt32(length);

            byte[] received = new byte[size];

            int bytesRead = 0;
            while (bytesRead < size)
            {
                int read = stream.Read(received, bytesRead, received.Length - bytesRead);
                bytesRead += read;
                //Console.WriteLine("ReadMessage: " + read);
            }
            return Encoding.ASCII.GetString(received);
        }

        private static void close()
        {
            stream.Close();
            client.Close();
        }
    }

}
