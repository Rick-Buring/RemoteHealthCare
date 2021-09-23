using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace RemoteHealthCare
{
    public class EquipmentMain : IDataListener
    {
        private DataIO dataIO;
        
        private Ergometer ergometer;
        private HeartBeatMonitor HeartBeatMonitor;
        private GUI gui;

       public static void Main(string[] args)
        {

        }
        // starts the application
        public async Task start()
        {
            dataIO = new DataIO();
            //ergometer = new Ergometer("Tacx Flux 00438", this, dataIO);
            ergometer = new ErgoSimulator(this);
            //this.gui = new GUI();
            await ergometer.Connect();
            

            //HeartBeatMonitor = new HeartBeatMonitor(this, dataIO);
            HeartBeatMonitor = new HBSimulator(this);
            await HeartBeatMonitor.Connect();


            Console.Read();
        }

        /// <summary>
        /// Deze methode wordt aangeroepen als er een callback is.
        /// </summary>
        /// <param name="data">Data in de vorm van IData.</param>
        public void notify(IData data)
        {
            if(HeartBeatMonitor != null)
            {
                Debug.WriteLine($"{ergometer.GetData()}\n{HeartBeatMonitor.GetData()}");
            }
           
        }
    }
    
}
