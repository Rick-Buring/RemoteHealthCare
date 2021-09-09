using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteHealthCare
{
    class ErgometerData : IData
    {
        //0x19 data page variables
        public int ID { get; private set; }
        public int Cadence { get; private set; }
        public int AccumulatedPower { get; private set; }
        public int InstantaneousPower { get; private set; }

        //0x10 data page variables
        public int ElapsedTime { get; private set; }
        public int DistanceTravelt { get; private set; }
        public double InstantaneousSpeed { get; private set; }

        public void Update(byte[] bytes)
        {
          switch( bytes[4]){
                case 0x10: decodeGeneralData(bytes);
                    break;
                case 0x19: decodeSpecificData(bytes);
                    break;
            }
        }

        private void decodeSpecificData(byte[] data)
        {
            // is the cadence
            Cadence = data[6];

            // is the accumulated power in Watts
            int accumulatedPowerLSB = data[7];
            int accumulatedPowerMSB = data[8];
            AccumulatedPower = (accumulatedPowerMSB << 8 | accumulatedPowerLSB);

            // is the instantaneous power in Watts
            int instantaneousPowerLSB = data[9];
            int instantaneousPowerMSB = data[10] & 0x0f << 8;
            InstantaneousPower = (instantaneousPowerMSB | instantaneousPowerLSB);

            // is the current trainer status
            ///*trainerStatus*/ = data[10] >> 4;

            // is for setting flags like target power limits
            //flagsField = data[11] & 0x0f;

            // is for the state is the fitness equipment
            //feStateField = data[11] >> 4;
        }

        private void decodeGeneralData(byte[] data)
        {
            //this.equipmentType = data[5];
            this.ElapsedTime = data[6];
            this.DistanceTravelt = data[7];

            int speedLSB = data[8];
            int speedMSB = data[9];
            this.InstantaneousSpeed = (speedMSB << 8 | speedLSB) / 1000.0;

            //this.capabilities = data[10] & 0x0f;
            //this.FEState = data[11];
        }

        internal string GetString()
        {
            StringBuilder builder = new StringBuilder();

            builder.Append("RPM: " + Cadence);
            builder.Append("\nAcc power: " + AccumulatedPower);
            builder.Append("\nInt power: " + InstantaneousPower);
            builder.Append("\nElapsed time (s): " + this.ElapsedTime / 4);
            builder.Append("\nDistance traveled: " + this.DistanceTravelt);
            builder.Append("\nSpeed (m/s): " + this.InstantaneousSpeed);
           

            return builder.ToString();
        }
    }
}
