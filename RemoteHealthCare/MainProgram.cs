using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RemoteHealthCare
{
    class MainProgram : IDataListener
    {

        private GUI gui;

        static async Task Main(string[] args)
        {
            MainProgram program = new MainProgram();
            await program.start();
        }

        private async Task start()
        {
            this.gui = new GUI();
            Ergometer bike = new Ergometer(this, "Tacx Flux 01140");
            await bike.Connect();

            HeartBeatMonitor hrm = new HeartBeatMonitor(this);
            await hrm.Connect();


            Console.Read();
        }

        private string bikeData = "";
        private string heartRateData = "";
        private string generalBikeData = "";

        private bool receivedBikeData = false;
        private bool receivedHeartRateData = false;
        private bool receivedGerenalData = false;

        public void notify(string data, int id)
        {
            switch (id)
            {
                case 0x19:
                    this.bikeData = data;
                    this.receivedBikeData = true;
                    break;
                case 0x16:
                    this.heartRateData = data;
                    this.receivedHeartRateData = true;
                    break;
                case 0x10:
                    this.generalBikeData = data;
                    this.receivedGerenalData = true;
                    break;
            }

            if (this.receivedBikeData && this.receivedHeartRateData && this.receivedGerenalData)
            {
                StringBuilder builder = new StringBuilder();
                builder.Append(this.bikeData);
                builder.Append("\nHeartrate (BPM): " + this.heartRateData);
                builder.Append("\n" + this.generalBikeData);

                this.gui.write(builder.ToString());
            }



        }
    }
    
}
