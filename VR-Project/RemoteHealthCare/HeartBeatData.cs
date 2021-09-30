using System;
using System.Collections.Generic;
using System.Text;

namespace Vr_Project.RemoteHealthcare
{
    public class HeartBeatData : IData
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

        internal int GetHeartRate ()
        {
            return this.HeartRate;
        }
    }
}
