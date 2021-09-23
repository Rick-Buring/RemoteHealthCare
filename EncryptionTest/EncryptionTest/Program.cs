using System;
using System.Threading;


namespace EncryptionTest
{
    class Program
    {
        private static string certificateName; 
        static void Main(string[] args)
        {
            certificateName = "";
           
            new Thread(startServer).Start();


            string serverCertificateName = "testCertificaat";
            string machineName = "";
            Client.RunClient(machineName, serverCertificateName);

        }

        private static void startServer()
        {
            Server.RunServer(certificateName);
        }

       
    }
}
