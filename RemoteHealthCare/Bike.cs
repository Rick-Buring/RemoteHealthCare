using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Avans.TI.BLE;

namespace RemoteHealthCare
{
    class Bike
    {
        private BLE bleBike;
        private Dictionary<int, IData> dataDict;
        private IDataListener listener;

        public Bike(IDataListener listener)
        {
            this.bleBike = new BLE();
            this.listener = listener;
            dataDict = new Dictionary<int, IData>();
            dataDict.Add(0x19, new BikeInfo());
            dataDict.Add(0x10, new GeneralBikeInfo());

            Thread.Sleep(1000); // We need some time to list available devices

            // List available devices
            List<String> bleBikeList = bleBike.ListDevices();
            Console.WriteLine("Devices found: ");
            foreach (var name in bleBikeList)
            {
                Console.WriteLine($"Device: {name}");
            }
        }

        public async Task connect(string name, bool realBike)
        {
            Console.WriteLine("connecting");
            if (!realBike)
            {
                throw new NotImplementedException("No Simulation created");
            }

            int errorCode = 1;
            // Connecting
            while (errorCode == 1)
            {
                errorCode = await bleBike.OpenDevice(name);
            }
            // __TODO__ Error check

            var services = bleBike.GetServices;
           
            foreach (var service in services)
            {
                Console.WriteLine($"Service: {service.Name}");
            }

            // Set service
            errorCode = 1;
            while (errorCode == 1)
            {
                errorCode = await bleBike.SetService("6e40fec1-b5a3-f393-e0a9-e50e24dcca9e");
                // __TODO__ error check
            }

            errorCode = 1;
            while (errorCode == 1)
            {
                // Subscribe
                bleBike.SubscriptionValueChanged += BleBike_SubscriptionValueChanged;
                errorCode = await bleBike.SubscribeToCharacteristic("6e40fec2-b5a3-f393-e0a9-e50e24dcca9e");
            }
        }

        private static byte[] sendResistanceMessage(float resistance)
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

        private void BleBike_SubscriptionValueChanged(object sender, BLESubscriptionValueChangedEventArgs e)
        {
            //  Console.WriteLine("Received from {0}: {1}, {2}", e.ServiceName,
                //BitConverter.ToString(e.Data).Replace("-", " "),
                //Encoding.UTF8.GetString(e.Data));
            if (dataDict.ContainsKey(e.Data[4]))
            {
                IData data = dataDict[e.Data[4]];
                data.Update(e.Data);
                switch (e.Data[4])
                {
                    case 0x10:
                        GeneralBikeInfo generalBikeInfo = dataDict[0x10] as GeneralBikeInfo;
                        this.listener.notify(generalBikeInfo.getData(), e.Data[4]);
                        break;
                    case 0x19:
                        BikeInfo bikeInfo = dataDict[0x19] as BikeInfo;
                        this.listener.notify(bikeInfo.getData(), e.Data[4]);
                        break;
                }

                
                
            }
        }

    }
}
