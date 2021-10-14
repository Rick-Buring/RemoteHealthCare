using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using VR_Project;

namespace Vr_Project.RemoteHealthcare
{
    public class EquipmentMain : IDataListener, System.IDisposable
    {
        private DataIO dataIO;

        public Ergometer ergometer { get; private set; }
        private HeartBeatMonitor heartBeatMonitor;

        // starts the application
        public async Task start(string bikeName, bool simulationChecked)
        {
            dataIO = new DataIO();
            if (!simulationChecked)
                ergometer = new Ergometer(bikeName, this, dataIO);
            else
                ergometer = new ErgoSimulator(this);
            ViewModel.resistanceUpdater += ergometer.SendResistance;
            //ergometer = new ErgoSimulator(this);

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
            if (heartBeatMonitor != null)
            {
                //Debug.WriteLine($"{ergometer.GetData()}\n{heartBeatMonitor.GetData()}");
                ViewModel.updater.Invoke(ergometer, heartBeatMonitor);
            }

        }

        public void Dispose()
        {
            if (ergometer != null)
                ergometer.Dispose();

        }
    }

}
