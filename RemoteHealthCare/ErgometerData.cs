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
        public int InstantaneousSpeed { get; private set; }

        public void Update(byte[] bytes)
        {
          switch( bytes[4]){
                case 0x10: decodeGeneralData(bytes);
                    break;
                case 0x19: decodeSpecificData(bytes);
                    break;
            }
        }

        private void decodeSpecificData(byte[] bytes)
        {

        }

        private void decodeGeneralData(byte[] bytes)
        {

        }

    }
}
