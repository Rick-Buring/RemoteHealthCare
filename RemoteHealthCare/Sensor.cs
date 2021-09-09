using Avans.TI.BLE;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RemoteHealthCare
{
    abstract class Sensor 
    {
        public abstract Task Connect();

        public abstract void SubscriptionValueChanged(object sender, BLESubscriptionValueChangedEventArgs e);

    }
}
