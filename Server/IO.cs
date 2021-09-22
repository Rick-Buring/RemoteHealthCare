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
            File.WriteAllText(Environment.CurrentDirectory + "/" + client, "a");

        }
    }
}
