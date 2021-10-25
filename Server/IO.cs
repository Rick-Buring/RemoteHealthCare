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

        /// <summary>
        /// writes data to a client's file
        /// </summary>
        /// <param name="client">the name of the client</param>
        /// <param name="data">the data to write</param>
        public void writeToFile(string client, HealthData data)
        {
            if (File.Exists($"{Environment.CurrentDirectory}/{client}.txt") && lastSesionTime(client) > data.ElapsedTime)
            {
                File.WriteAllText($"{Environment.CurrentDirectory}/{client}.txt", "");
            }

            StreamWriter writer = File.AppendText($"{Environment.CurrentDirectory}/{client}.txt");
            try
            {
                writer.WriteLine(data.ToString());  
            } catch (FileNotFoundException e)
            {
                Console.WriteLine(e.StackTrace);
            } finally
            {
                writer.Close();
            }
        }

        /// <summary>
        /// method used for recieving all text from a client's file
        /// </summary>
        /// <param name="clientName">the name of the client</param>
        /// <returns>the text in the file</returns>
        public string getText(string client)
        {
            return File.ReadAllText($"{Environment.CurrentDirectory}/{client}.txt");
        }

        private int lastSesionTime(string client)
        {
            string[] content = File.ReadAllLines($"{Environment.CurrentDirectory}/{client}.txt");
            string[] lastInput = content[content.Length - 1].Split(",");
            int result;
            if (int.TryParse(lastInput[5], out result)) return result;
            else return -1;
        }
    }
}
