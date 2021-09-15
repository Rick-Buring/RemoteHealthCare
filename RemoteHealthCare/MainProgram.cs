using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RemoteHealthCare
{
    class MainProgram : IDataListener
    {
        private DataIO dataIO;

        private Ergometer ergometer;
        private HeartBeatMonitor HeartBeatMonitor;
        private GUI gui;

        static async Task Main(string[] args)
        {
            MainProgram program = new MainProgram();
            await program.start();
        }

        private async Task start()
        {
            dataIO = new DataIO();
           // ergometer = new Ergometer("Tacx Flux 01249", this, dataIO);
            ergometer = new ErgoSimulator(this);
            this.gui = new GUI();
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
                gui.write($"{ergometer.GetData()}\n{HeartBeatMonitor.GetData()}");
            }
           
        }
    }
    
}
