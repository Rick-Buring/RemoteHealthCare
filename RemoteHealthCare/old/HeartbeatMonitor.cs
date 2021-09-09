using Avans.TI.BLE;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RemoteHealthCare
{
    class HeartbeatMonitor
    {
        private BLE bleHeart;
        private Heartrate heartrate;
        private IDataListener listener;

        public HeartbeatMonitor(IDataListener listener)
        {
            this.listener = listener;
            this.bleHeart = new BLE();
            heartrate = new Heartrate();
           
            Console.Read();
        }

        public async Task connect(bool realMonitor)
        {
            if (!realMonitor)
            {
                throw new NotImplementedException("No Simulation created");
            }

            int errorCode;

            errorCode = await bleHeart.OpenDevice("Decathlon Dual HR");

            await bleHeart.SetService("HeartRate");

            bleHeart.SubscriptionValueChanged += SubscriptionValueChanged;
            await bleHeart.SubscribeToCharacteristic("HeartRateMeasurement");

        }

        private void SubscriptionValueChanged(object sender, BLESubscriptionValueChangedEventArgs e)
        {
            Console.WriteLine("Received from {0}: {1}, {2}", e.ServiceName,
                BitConverter.ToString(e.Data).Replace("-", " "),
                Encoding.UTF8.GetString(e.Data));

            //heartrate.Update(e.Data);
        }

    }
}
