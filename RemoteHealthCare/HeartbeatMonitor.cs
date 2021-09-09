using Avans.TI.BLE;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RemoteHealthCare
{
    class HeartBeatMonitor : Sensor
    {
        private BLE bleHeart;
        private HeartBeatData heartBeatData;
        private IDataListener[] listeners;

        public HeartBeatMonitor(params IDataListener[] listener)
        {
            this.listeners = listener;
            this.heartBeatData = new HeartBeatData();
            this.bleHeart = new BLE();
        }

        public override async Task Connect()
        {
            int errorCode;

            errorCode = await bleHeart.OpenDevice("Decathlon Dual HR");

            await bleHeart.SetService("HeartRate");

            bleHeart.SubscriptionValueChanged += SubscriptionValueChanged;
            await bleHeart.SubscribeToCharacteristic("HeartRateMeasurement");
        }

        public override void SubscriptionValueChanged(object sender, BLESubscriptionValueChangedEventArgs e)
        {
            Console.WriteLine("Received from {0}: {1}, {2}", e.ServiceName,
               BitConverter.ToString(e.Data).Replace("-", " "),
               Encoding.UTF8.GetString(e.Data));

            heartBeatData.Update(e.Data);
           
        }

        private void notifyListeners()
        {
            for (int i = 0; i < this.listeners.Length; i++)
            {
                this.listeners[i].notify(heartBeatData);
            }
        }

    }
}
