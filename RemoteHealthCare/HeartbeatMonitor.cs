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
        //private Heartrate heartrate;
        private IDataListener listener;

        public HeartBeatMonitor(IDataListener listener)
        {
            this.listener = listener;
            this.bleHeart = new BLE();
            //heartrate = new Heartrate();
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

        public override void SubscriptionValueChanged(object sender, BLESubscriptionValueChangedEventArgs e)
        {
            //Console.WriteLine("Received from {0}: {1}, {2}", e.ServiceName,
            //   BitConverter.ToString(e.Data).Replace("-", " "),
            //   Encoding.UTF8.GetString(e.Data));

            //heartrate.Update(e.Data);

            this.listener.notify(e.Data[1].ToString(), 0x16);

        }

    }
}
