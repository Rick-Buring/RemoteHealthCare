using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Vr_Project.RemoteHealthcare
{
    public class ErgometerData : IData
    {
        //0x19 data page variables
        public int ID { get; set; }
        public int Cadence { get; set; }
        public int AccumulatedPower { get; set; }
        public int InstantaneousPower { get; set; }

        //0x10 data page variables
        public int ElapsedTime { get; set; }
        private int timeRollovers = 0;
        private int oldTime = 0;
        private readonly int rolloverTime = 64;
        public int DistanceTraveled { get; set; }
        private int distanceRollovers = 0;
        private int oldDistance = 0;
        private readonly int rolloverDistance = 256;
        public double InstantaneousSpeed { get; set; }

        //updates de variabelen
        public void Update(byte[] bytes)
        {
          switch( bytes[4]){
                case 0x10: decodeGeneralData(bytes);
                    break;
                case 0x19: decodeSpecificData(bytes);
                    break;
            }
            Debug.WriteLine(Cadence);
        }

        /// <summary>
        /// decoderen van de 0x19 page data
        /// </summary>
        /// <param name="data"></param>
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
            int instantaneousPowerMSB = (data[10] & 0x0f) << 8;
            InstantaneousPower = (instantaneousPowerMSB | instantaneousPowerLSB);

            // is the current trainer status
            ///*trainerStatus*/ = data[10] >> 4;

            // is for setting flags like target power limits
            //flagsField = data[11] & 0x0f;

            // is for the state is the fitness equipment
            //feStateField = data[11] >> 4;
        }

        /// <summary>
        /// decoderen van de 0x10 data page
        /// </summary>
        /// <param name="data"></param>
        private void decodeGeneralData(byte[] data)
        {
            //this.equipmentType = data[5];
            int newTime = data[6] / 4;
            if (newTime < this.oldTime) this.timeRollovers++;
            this.oldTime = newTime;
            this.ElapsedTime = this.timeRollovers * this.rolloverTime + newTime;

            int newDistance = data[7];
            if (newDistance < this.oldDistance) this.distanceRollovers++;
            this.oldDistance = newDistance;
            this.DistanceTraveled = this.distanceRollovers * this.rolloverDistance + newDistance;


            int speedLSB = data[8];
            int speedMSB = data[9];
            //TODO fix calculation
            this.InstantaneousSpeed = (((speedMSB << 8) | speedLSB) / 1000) * 3.6;


            //this.capabilities = data[10] & 0x0f;
            //this.FEState = data[10] >> 4;
        }

        internal string GetString()
        {
            StringBuilder builder = new StringBuilder();

            builder.Append("RPM: " + Cadence);
            builder.Append("\nAcc power: " + AccumulatedPower);
            builder.Append("\nInt power: " + InstantaneousPower);
            builder.Append("\nElapsed time (s): " + this.ElapsedTime / 4);
            builder.Append("\nDistance traveled: " + this.DistanceTraveled);
            builder.Append("\nSpeed (m/s): " + this.InstantaneousSpeed);
           

            return builder.ToString();
        }

        internal ErgometerData getData ()
        {
            return this;
        }
    }
}
