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

        //HeartBeatData heartBeatData = new HeartBeatData();

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


                setData();
                listener.notify(base.heartBeatData);
                Thread.Sleep(1000);
            }
        }

        public void setData()
        {
            Random random = new Random();
            base.heartBeatData.HeartRate = baseline + random.Next(baseline / 5);
        }

        public IData GetData()
        {
            return base.heartBeatData;
        }

    }
}
