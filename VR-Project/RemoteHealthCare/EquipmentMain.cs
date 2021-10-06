using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using VR_Project;

namespace Vr_Project.RemoteHealthcare
{
    public class EquipmentMain : IDataListener, IDisposable
    {
        private DataIO dataIO;
        
        public Ergometer ergometer { get; private set; }
        private HeartBeatMonitor heartBeatMonitor;
        private ViewModel.Update updater;
        
        public EquipmentMain (ViewModel.Update updater)
        {
            this.updater = updater;
        }
        // starts the application
        public async Task start()
        {
            dataIO = new DataIO();
            //ergometer = new Ergometer("Tacx Flux 01249", this, dataIO);
            ergometer = new ErgoSimulator(this);
            //this.gui = new GUI();
            await ergometer.Connect();
            

            //heartBeatMonitor = new HeartBeatMonitor(this, dataIO);
            heartBeatMonitor = new HBSimulator(this);
            await heartBeatMonitor.Connect();


            Console.Read();
        }

        /// <summary>
        /// Deze methode wordt aangeroepen als er een callback is.
        /// </summary>
        /// <param name="data">Data in de vorm van IData.</param>
        public void notify(IData data)
        {
            if(heartBeatMonitor != null)
            {
                //Debug.WriteLine($"{ergometer.GetData()}\n{heartBeatMonitor.GetData()}");
                this.updater.Invoke(ergometer, heartBeatMonitor);
            }
           
        }

        public void Dispose()
        {   if (ergometer != null)
            ergometer.Dispose();

        }
    }
    
}
