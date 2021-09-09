using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteHealthCare
{
    class HeartBeatData : IData
    {
        public int HeartRate{ get; set; }

        public void Update(byte[] bytes)
        {
            this.HeartRate = bytes[1];
        }

        internal string GetString()
        {
            return $"Heartrate: {HeartRate}";
        }
    }
}
