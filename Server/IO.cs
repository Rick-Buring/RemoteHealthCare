using CommunicationObjects.DataObjects;
using System;
using System.IO;

namespace Server
{
    public class IO
    {
        private string filePath;
        public IO(string filePath)
        {
            this.filePath = filePath;
        }

        /// <summary>
        /// writes data to a client's file
        /// </summary>
        /// <param name="client">the name of the client</param>
        /// <param name="data">the data to write</param>
        internal void writeToFile(string client, HealthData data)
        {
            StreamWriter writer = File.AppendText($"{filePath}/{client}.txt");
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
        public string getText(string clientName)
        {
            return File.ReadAllText($"{Environment.CurrentDirectory}/{clientName}.txt");
        }

    }
}
