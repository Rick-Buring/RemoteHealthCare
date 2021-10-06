using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Vr_Project.RemoteHealthcare
{

    public class ErgoSimulator : Ergometer
    {
        private IDataListener listener;

        private int baseline;

        public int rpm { get; set; }
        public int accumulatedTotal { get; set; }
        public int instantaneousTotal { get; set; }
        public int id { get; set; }
        public int elapsedTime { get; set; }
        public int distanceTraveled { get; set; }

        public ErgoSimulator(params IDataListener[] listener) : base("Simulation", listener)
        {

            this.rpm = 0;
            this.accumulatedTotal = 0;
            this.instantaneousTotal = 0;
            this.id = 0;
            this.elapsedTime = 0;
            this.distanceTraveled = 0;

            this.listener = listener[0];

        }

        //Methode voor het verbinden met de simulator.
        public override async Task Connect()
        {
            Console.WriteLine($"Connecting to {Name}");

            //Start een thread die rollBaseLine uitvoert.
            Thread thread = new Thread(new ThreadStart(rollBaseline));
            thread.Start();

        }

        //Rolt elke seconde een random waarde en verandert de baseline gebaseerd op die waarde met +10 of -10.
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

                //Baseline mag niet lager zijn dan 40 en niet hoger zijn dan 120.
                baseline = Math.Clamp(baseline, 40, 120);

                //Verander de waarden in de data klasse en geef notify de listener.
                setValues();
                listener.notify(base.ergometerData);
                Thread.Sleep(1000);
            }
            
        }

        //Verandert de waardes in de data klasse met een random waarde rond de baseline plus een random waarde tussen 0 en een aantal % van de baseline.
        private void setValues()
        {

            Random random = new Random();

            //Baseline + (tussen 0 en 10% van de baseline)
            this.rpm = baseline + random.Next(baseline / 10);

            //80 + (tussen 0 en 5% van de baseline)
            this.accumulatedTotal += (80 + random.Next(baseline / 20));

            //100 + (tussen 0 en 20% van de baseline)
            this.instantaneousTotal = 100 + random.Next(baseline / 5);

            base.ergometerData.ID = this.id;

            //Baseline + (tussen 0 en 10% van de baseline)
            base.ergometerData.Cadence = baseline + random.Next(baseline / 10);

            //AccumulatedPower += 80 + (tussen 0 en 5% van de baseline)
            base.ergometerData.AccumulatedPower += (80 + random.Next(baseline / 20));

            //InstantaneousPower += 100 + (tussen 0 en 20% van de baseline)
            base.ergometerData.InstantaneousPower = 100 + random.Next(baseline / 5);

            base.ergometerData.ElapsedTime = this.elapsedTime;
            base.ergometerData.DistanceTraveled = this.distanceTraveled;

            //20 + (tussen 0 en 25% van de baseline)
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
