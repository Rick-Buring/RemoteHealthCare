using Avans.TI.BLE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

            Thread.Sleep(1000); // We need some time to list available devices
                                // List available devices
            List<String> bleBikeList = bleHeart.ListDevices();
            Console.WriteLine("Devices found: ");
            foreach (var name in bleBikeList)
            {
                Console.WriteLine($"Device: {name}");
            }
        }

        public async Task connect(bool realMonitor)
        {
            if (!realMonitor)
            {
                throw new NotImplementedException("No Simulation created");
            }

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

        private void SubscriptionValueChanged(object sender, BLESubscriptionValueChangedEventArgs e)
        {
            //Console.WriteLine("Received from {0}: {1}, {2}", e.ServiceName,
            //BitConverter.ToString(e.Data).Replace("-", " "),
            //Encoding.UTF8.GetString(e.Data));

            //heartrate.Update(e.Data);

            // IData data = dataDict[e.Data[4]];

            heartrate.Update(e.Data);
            
            this.listener.notify(heartrate.getData(), 0x16);
        }

    }
}
