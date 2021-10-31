using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Vr_Project.RemoteHealthcare
{
    public class HeartBeatData : BindableBase, IData, INotifyPropertyChanged
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
