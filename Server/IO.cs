using CommunicationObjects.DataObjects;
using System;
using System.IO;

namespace Server
{
    public class IO
    {
        public IO()
        {

        }

        internal void writeToFile(string client, HealthData data)
        {
            
            StreamWriter writer = File.AppendText($"{Environment.CurrentDirectory}/{client}.txt");
            writer.WriteLine(data.ToString());
            writer.Close();
        }
    }
}
