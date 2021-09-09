using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteHealthCare
{
    class Heartrate : IData
    {

        private int heartBeat;

        public Heartrate ()
        {
            this.heartBeat = 0;
        }
        public void Update(byte[] bytes)
        {
            this.heartBeat = bytes[1];
        }

        public string getData ()
        {
            return this.heartBeat.ToString();
        }
    }
}
