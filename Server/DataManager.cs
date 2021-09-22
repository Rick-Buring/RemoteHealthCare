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

        internal void write(string sender, HealthData data)
        {
            this.io.writeToFile(sender, data);
        }
    }
}
