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
            Bike bike = new Bike(this);
            await bike.connect("Tacx Flux 01140", true);

            HeartbeatMonitor hrm = new HeartbeatMonitor(this);
            await hrm.connect(true);


            Console.Read();
        }




        public void notify(string data)
        {
            throw new NotImplementedException();
        }
    }
}
