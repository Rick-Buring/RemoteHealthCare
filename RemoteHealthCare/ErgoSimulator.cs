using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RemoteHealthCare
{

    class ErgoSimulator : Ergometer
    {

        private string Name;
        private IDataListener listener;

        private int baseline;

        private int rpm;
        private int accumulatedTotal;
        private int instantaneousTotal;
        private int id = 0;
        private int elapsedTime = 0;
        private int distanceTraveled = 0;

        //ErgometerData ergometerData = new ErgometerData();

        public ErgoSimulator(params IDataListener[] listener) : base("Simulation", listener)
        {

            this.listener = listener[0];
            this.Name = base.Name;

        }

        public override async Task Connect()
        {
            Console.WriteLine($"Connecting to {Name}");

            Thread thread = new Thread(new ThreadStart(rollBaseline));
            thread.Start();

        }

        private void rollBaseline()
        {

            bool running = true;

            while(running)
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

                baseline = Math.Clamp(baseline, 40, 120);


                setValues();
                listener.notify(base.ergometerData);
                Thread.Sleep(1000);
            }

        }

        private void setValues()
        {

            Random random = new Random();
            this.rpm = baseline + random.Next(baseline / 10);
            this.accumulatedTotal += (80 + random.Next(baseline / 20));
            this.instantaneousTotal = 100 + random.Next(baseline / 5);

            base.ergometerData.ID = this.id;
            base.ergometerData.Cadence = baseline + random.Next(baseline / 10);
            base.ergometerData.AccumulatedPower += (80 + random.Next(baseline / 20));
            base.ergometerData.InstantaneousPower = 100 + random.Next(baseline / 5);

            base.ergometerData.ElapsedTime = this.elapsedTime;
            base.ergometerData.DistanceTraveled = this.distanceTraveled;
            base.ergometerData.InstantaneousSpeed = 20 + random.Next(baseline / 4);

            this.id++;
            this.elapsedTime++;
            this.distanceTraveled += 2;
        }

        public IData GetData()
        {
            return base.ergometerData;
        }

    }
}
