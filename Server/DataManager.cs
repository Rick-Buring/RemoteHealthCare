using CommunicationObjects.DataObjects;
using System;
namespace Server
{
    public class DataManager
    {
        private IO io;

        public DataManager()
        {
            this.io = new IO();
        }

        /// <summary>
        /// used to tell the IO class to write data to the client's file
        /// </summary>
        /// <param name="sender">the name of the client to who's file the data must be written</param>
        /// <param name="data">the data to write to the file</param>
        internal void write(string sender, HealthData data)
        {
            this.io.writeToFile(sender, data);
        }

        /// <summary>
        /// method for getting the history of a client in text form
        /// </summary>
        /// <param name="clientName">the client's name who's history must be recieved</param>
        /// <returns>the history in text form</returns>
        internal string GetHistory(string clientName)
        {
            return this.io.getText(clientName);
        }
    }
}
