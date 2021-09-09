using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RemoteHealthCare
{
    class MainProgram : IDataListener
    {
        static async Task Main(string[] args)
        {
            MainProgram program = new MainProgram();
            await program.start();
        }

        private async Task start()
        {
            Ergometer bike = new Ergometer(this, "Tacx Flux 01140");
            await bike.Connect();

            HeartBeatMonitor hrm = new HeartBeatMonitor(this);
            await hrm.Connect();


            Console.Read();
        }

        public void notify(string data)
        {
            throw new NotImplementedException();
        }
    }
}
