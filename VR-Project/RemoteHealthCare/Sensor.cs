using Avans.TI.BLE;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;

namespace Vr_Project.RemoteHealthcare
{
    public abstract class Sensor : BindableBase, INotifyPropertyChanged
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
        public abstract int GetHeartBeat();

        public abstract ErgometerData GetErgometerData();
        public abstract byte[] ResistanceMessage(float resistance);
        public abstract void SendResistance(float resistance);
    }
}
