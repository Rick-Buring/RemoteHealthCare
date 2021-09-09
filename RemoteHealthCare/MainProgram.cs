using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RemoteHealthCare
{
    class MainProgram : IDataListener
    {
        private DataIO dataIO;


        private GUI gui;

        static async Task Main(string[] args)
        {
            MainProgram program = new MainProgram();
            await program.start();
        }

        private async Task start()
        {
            dataIO = new DataIO();
            Ergometer bike = new Ergometer("Tacx Flux 01140", this, dataIO);
            this.gui = new GUI();
            await bike.Connect();

            HeartBeatMonitor hrm = new HeartBeatMonitor(this, dataIO);
            await hrm.Connect();


            Console.Read();
        }

        public void notify(IData data)
        {

        }
    }
    
}
