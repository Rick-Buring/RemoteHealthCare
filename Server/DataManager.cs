using CommunicationObjects.DataObjects;
using System;
using System.Collections.Generic;
using System.IO;

namespace Server
{
    public class DataManager
    {
        private IO io;
        public static string appDataHealthCareDirectory;

        public DataManager()
        {
            this.io = new IO(appDataHealthCareDirectory);
        }

        public static void initFoldersAndFilePath()
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            

            appDataHealthCareDirectory = appDataPath + @"\RemoteHealthCare";
            if (!Directory.Exists(appDataHealthCareDirectory))
            {
                Directory.CreateDirectory(appDataHealthCareDirectory);
            }
            string appDataPasswordFolder = appDataHealthCareDirectory + @"\Passwords";

            if (!Directory.Exists(appDataPasswordFolder))
            {
                Directory.CreateDirectory(appDataPasswordFolder);
            }

            string appDataClientsinfo = appDataHealthCareDirectory + @"\ClientsInfo";

            if (!Directory.Exists(appDataClientsinfo))
            {
                Directory.CreateDirectory(appDataClientsinfo);
            }
        }

        public static string[] ReturnClientsFromInfoFolder()
        {
            String[] filesInDirectory = Directory.GetFiles(appDataHealthCareDirectory + @"\ClientsInfo");
            List<String> fileNameHolder = new List<string>();
            foreach (var directoryFile in filesInDirectory)
            {
                //TODO change
                if (directoryFile.EndsWith(".txt"))
                {
                    string file = directoryFile.Substring(directoryFile.LastIndexOf(@"\") + 1);
                    fileNameHolder.Add(file.Remove(file.Length - 4));
                }
            }

            return fileNameHolder.ToArray();
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
