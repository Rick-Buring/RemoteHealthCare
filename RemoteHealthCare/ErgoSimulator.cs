using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RemoteHealthCare
{

    class ErgoSimulator : Ergometer
    {

        private string Name;
        private IDataListener listener;

        private int baseline;

        public ErgoSimulator(IDataListener listener) : base(listener, "Simulation")
        {

            this.listener = listener;
            this.Name = base.Name;

        }

        public override async Task Connect()
        {
            Console.WriteLine("Connecting to {Name}", Name);
        }

        private void rollBaseline()
        {
            Random random = new Random();
            if(random.Next(100) >= 50)
            {
                baseline += 10;
            }
        }

    }
}
