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
            try
            {
                writer.WriteLine(data.ToString());  
            } catch
            {
                // __CR__ [PSMG] Catch nooit alle exception en doe er dan niks mee!
            } finally
            {
                writer.Close();
            }
        }
    }
}
