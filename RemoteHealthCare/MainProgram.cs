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
            Ergometer bike = new ErgoSimulator(this);
            //new Ergometer(this, "Tacx Flux 01140");
            await bike.Connect();

            HeartBeatMonitor hrm = new HBSimulator(this);
            //new HeartBeatMonitor(this);
            await hrm.Connect();


            Console.Read();
        }

        public void notify(IData data)
        {

        }
    }
    
}
