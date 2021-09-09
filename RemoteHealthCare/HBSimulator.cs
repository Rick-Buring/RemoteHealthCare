using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RemoteHealthCare
{
    class HBSimulator : HeartBeatMonitor
    {

        private IDataListener listener;

        private int baseline;

        public HBSimulator(IDataListener listener) : base(listener)
        {

            this.listener = listener;

        }

        public async override Task Connect()
        {
            Console.WriteLine($"Connecting to HB Simulator");

            Thread thread = new Thread(new ThreadStart(rollBaseline));
            thread.Start();
        }

        public void rollBaseline()
        {
            bool running = true;

            while (running)
            {
                Random random = new Random();
                if (random.Next(100) >= 50)
                {
                    baseline += 10;
                }
                else
                {
                    baseline -= 10;
                }

                baseline = Math.Clamp(baseline, 60, 180);

                Thread.Sleep(1000);
                sendData();
            }
        }

        public void sendData()
        {
            Random random = new Random();
            listener.notify((baseline + random.Next(baseline / 5)).ToString());
        }


    }
}
