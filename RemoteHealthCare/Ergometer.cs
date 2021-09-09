using Avans.TI.BLE;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RemoteHealthCare
{
    class Ergometer : Sensor
    {
        public string Name { get; }

        private IDataListener listener;
        private BLE bleBike;

        public Ergometer(IDataListener listener, string name)
        {
            this.Name = name;
            this.bleBike = new BLE();
            this.listener = listener;

            Thread.Sleep(1000); // We need some time to list available devices

            // List available devices
            List<String> bleBikeList = bleBike.ListDevices();
            Console.WriteLine("Devices found: ");
            foreach (var deviceName in bleBikeList)
            {
                Console.WriteLine($"Device: {deviceName}");
            }
        }

        public override async Task Connect()
        {
            Console.WriteLine("connecting");

            int errorCode;
            // Connecting
            errorCode = await bleBike.OpenDevice(Name);
            // __TODO__ Error check

            var services = bleBike.GetServices;
            foreach (var service in services)
            {
                Console.WriteLine($"Service: {service.Name}");
            }

            // Set service
            errorCode = await bleBike.SetService("6e40fec1-b5a3-f393-e0a9-e50e24dcca9e");
            // __TODO__ error check


            // Subscribe
            bleBike.SubscriptionValueChanged += SubscriptionValueChanged;
            errorCode = await bleBike.SubscribeToCharacteristic("6e40fec2-b5a3-f393-e0a9-e50e24dcca9e");
        }

        public override void SubscriptionValueChanged(object sender, BLESubscriptionValueChangedEventArgs e)
        {
            Console.WriteLine("Received from {0}: {1}, {2}", e.ServiceName, 
                BitConverter.ToString(e.Data).Replace("-", " "),
                Encoding.UTF8.GetString(e.Data));
        }

        private static byte[] ResistanceMessage(float resistance)
        {
            byte[] buff = new byte[13];
            //head
            buff[0] = 0xa4; //sync byte
            buff[1] = 0x09; //length data size +1 (broadcast is usualy 9 bytes long)
            buff[2] = 0x4e; // message type
            buff[3] = 0x05; //channel
            buff[4] = 0x30; //page

            //data
            buff[11] = (byte)(resistance * 2.0f);

            //checksum
            buff[12] = checksum(buff);

            return buff;
        }

        private static byte checksum(byte[] message)
        {
            byte output = message[0];

            for (int i = 1; i < message.Length - 1; i++)
            {
                output = (byte)(output ^ message[i]);
            }

            return output;
        }
    }
}
