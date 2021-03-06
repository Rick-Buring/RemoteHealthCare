using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Vr_Project.RemoteHealthcare
{
    public class HBSimulator : HeartBeatMonitor
    {

        private IDataListener listener;
        private int baseline;

        public HBSimulator(IDataListener listener) : base(listener)
        {

            base.heartBeatData = new HeartBeatData();
            this.listener = listener;

        }

        public override int GetHeartBeat()
        {
            return this.heartBeatData.HeartRate;
        }

        //Methode voor het verbinden met de simulator.
        public async override Task Connect()
        {
            Console.WriteLine($"Connecting to HB Simulator");

            //Start een thread die rollBaseLine uitvoert.
            Thread thread = new Thread(new ThreadStart(rollBaseline));
            thread.Start();
        }

        //Rolt elke seconde een random waarde en verandert de baseline gebaseerd op die waarde met +10 of -10.
        bool running = true;
        public void rollBaseline()
        {
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

                //baseline mag niet lager zijn dan 60 en niet hoger zijn dan 180.
                baseline = Math.Clamp(baseline, 60, 180);


                setData();
                listener.notify(base.heartBeatData);
                Thread.Sleep(1000);
            }
            
        }

        //Verandert de data in de data klasse.
        public void setData()
        {
            Random random = new Random();

            //baseline + (tussen 0 en 20% van de baseline)
            base.heartBeatData.HeartRate = baseline + random.Next(baseline / 5);
        }

        public IData GetData()
        {
            return base.heartBeatData;
        }

        public override void Dispose()
        {
            this.running = false;
        }

    }
}
