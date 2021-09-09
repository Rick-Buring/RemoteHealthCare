using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteHealthCare
{
    class BikeInfo : IData

    {

        private int rpm;
        private int accumulatedPower;
        private int instantaneousPower;
        private int trainerStatus;
        private int flagsField;
        private int feStateField;

        public BikeInfo ()
        {
            this.rpm = 0xff;
            this.accumulatedPower = 0xff;
            this.instantaneousPower = 0xff;
            this.trainerStatus = 0xff;
            this.flagsField = 0xff;
            this.feStateField = 0xff;
        }


        public void Update(byte[] bytes)
        {
            //throw new NotImplementedException();

            convertData(bytes);
            
        }

        /// <summary>
        /// This method is used to convert data from data page 0x19 to readable data.
        /// </summary>
        /// <param name="data"> is a byte array of data.</param>
        private void convertData(byte[] data)
        {


            // is the cadence
            rpm = data[6];

            // is the accumulated power in Watts
            int accumulatedPowerLSB = data[7];
            int accumulatedPowerMSB = data[8];
            accumulatedPower = (accumulatedPowerMSB << 8 | accumulatedPowerLSB);

            // is the instantaneous power in Watts
            int instantaneousPowerLSB = data[9];
            int instantaneousPowerMSB = data[10] & 0x0f << 8;
            instantaneousPower = (instantaneousPowerMSB | instantaneousPowerLSB);

            // is the current trainer status
            trainerStatus = data[10] >> 4;

            // is for setting flags like target power limits
            flagsField = data[11] & 0x0f;

            // is for the state is the fitness equipment
            feStateField = data[11] >> 4;



            // is to be used later as an output via callback.
            string output = build0x19String(rpm, accumulatedPower, instantaneousPower, trainerStatus, flagsField, feStateField);
            //subscriber.update();

            Console.Clear();
            Console.WriteLine(output);


        }

        private string build0x19String(int rpm, int accumulatedTotal, int instantaneousTotal,
                                        int trainerStatus, int flagsField, int feStateField)
        {
            StringBuilder builder = new StringBuilder();

            builder.Append("RPM: " + rpm);
            builder.Append("\nAcc power: " + accumulatedTotal);
            builder.Append("\nInt power: " + instantaneousTotal);
            builder.Append("\nTrainer status: " + trainerStatus);
            builder.Append("\nFlags field: " + flagsField);
            builder.Append("\nFEState: " + feStateField);

            return builder.ToString();

        }

        public string getData()
        {
            return build0x19String(this.rpm, this.accumulatedPower, this.instantaneousPower, this.trainerStatus, this.flagsField, this.feStateField);
        }

        


    }
}
