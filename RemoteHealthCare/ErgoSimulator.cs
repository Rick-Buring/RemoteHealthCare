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
        private int trainerStatus = 1;
        private int flagsField = 1;
        private int feStateField = 1;

        public ErgoSimulator(IDataListener listener) : base(listener, "Simulation")
        {

            this.listener = listener;
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

                Thread.Sleep(1000);
                sendData();
            }

        }

        private void setValues()
        {

            Random random = new Random();
            this.rpm = baseline + random.Next(baseline / 10);
            this.accumulatedTotal += (80 + random.Next(baseline / 20));
            this.instantaneousTotal = 100 + random.Next(baseline / 5);

        }

        private void sendData()
        {
            setValues();

            StringBuilder builder = new StringBuilder();
            builder.Append("RPM: " + this.rpm);
            builder.Append("\nAcc power: " + this.accumulatedTotal);
            builder.Append("\nInt power: " + this.instantaneousTotal);
            builder.Append("\nTrainer status: " + this.trainerStatus);
            builder.Append("\nFlags field: " + this.flagsField);
            builder.Append("\nFEState: " + this.feStateField);

            listener.notify(builder.ToString());
        }

    }
}
