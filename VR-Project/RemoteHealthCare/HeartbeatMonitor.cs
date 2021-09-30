using Avans.TI.BLE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Vr_Project.RemoteHealthcare
{
    public class HeartBeatMonitor : Sensor
    {
        private BLE bleHeart;
        protected HeartBeatData heartBeatData;
        private IDataListener[] listeners;

        public HeartBeatMonitor(params IDataListener[] listener)
        {
            this.listeners = listener;
            this.heartBeatData = new HeartBeatData();
            this.bleHeart = new BLE();
        }

        public override async Task Connect()
        {
            int errorCode = 1;
            while (errorCode == 1)
            {
                errorCode = await bleHeart.OpenDevice("Decathlon Dual HR");
                Console.WriteLine("Opening device HR: " + errorCode);
            }


            errorCode = 1;
            while (errorCode == 1)
            {
                errorCode = await bleHeart.SetService("HeartRate");
                Console.WriteLine("Setting service HR: " + errorCode);
            }


            
            bleHeart.SubscriptionValueChanged += SubscriptionValueChanged;
            errorCode = 1;

            while (errorCode == 1)
            {
                errorCode = await bleHeart.SubscribeToCharacteristic("HeartRateMeasurement");
                Console.WriteLine("Subscribing to characteristic HR: " + errorCode);
            }
        }

        public override int GetHeartBeat()
        {
            return this.heartBeatData.HeartRate;
        }

        public override void SubscriptionValueChanged(object sender, BLESubscriptionValueChangedEventArgs e)
        {
            heartBeatData.Update(e.Data);
            notifyListeners();
        }

        /// <summary>
        /// Deze methode wordt gebruikt om alle listeners aan te roepen.
        /// </summary>
        private void notifyListeners()
        {
            for (int i = 0; i < this.listeners.Length; i++)
            {
                this.listeners[i].notify(heartBeatData);
            }
        }

        public override ErgometerData GetErgometerData()
        {
            return null;
        }
    }
}
