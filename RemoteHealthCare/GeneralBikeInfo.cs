using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteHealthCare
{
    class GeneralBikeInfo : IData
    {

        private int equipmentType;
        private double elapsedTime;
        private int distanceTraveled;
        private double speed;
        private int capabilities;
        private int FEState;

        public GeneralBikeInfo ()
        {
            equipmentType = 0;
            elapsedTime = 0;
            distanceTraveled = 0;
            speed = 0;
            capabilities = 0;
            FEState = 0;
        }
        public void Update(byte[] bytes)
        {
            //throw new NotImplementedException();
            convertData(bytes);
        }

        private void convertData (byte[] data)
        {
            this.equipmentType = data[5];
            this.elapsedTime = data[6];
            this.distanceTraveled = data[7];

            int speedLSB = data[8];
            int speedMSB = data [9];
            this.speed = (speedMSB << 8 | speedLSB) / 1000.0;

            this.capabilities = data[10] & 0x0f;
            this.FEState = data[11];
        }

        private string buildString ()
        {
            StringBuilder builder = new StringBuilder();

            builder.Append("Elapsed time (s): " + this.elapsedTime / 4);
            builder.Append("\nDistance traveled: " + this.distanceTraveled);
            builder.Append("\nSpeed (m/s): " + this.speed);
            

            return builder.ToString();

        }

        public string getData()
        {
            return buildString();
        }
    }
}
