using Avans.TI.BLE;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RemoteHealthCare
{
    public abstract class Sensor 
    {
        /// <summary>
        /// Deze methode zal worden gebruikt om te verbinden met een sensor.
        /// </summary>
        /// <returns></returns>
        public abstract Task Connect();

        /// <summary>
        /// De methode wordt aangeroepen als er een waarde veranderd op een subscription.
        /// </summary>
        /// <param name="sender">Is the sender</param>
        /// <param name="e">Is the event arguments</param>
        public abstract void SubscriptionValueChanged(object sender, BLESubscriptionValueChangedEventArgs e);

        /// <summary>
        /// Deze methode zal worden gebruikt om de data van een sensor geven. 
        /// </summary>
        /// <returns>String</returns>
        public abstract string GetData();
      
    }
}
