using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Avans.TI.BLE;

namespace RemoteHealthCare__Framework_4._7._2_
{
    class Program
    {
        private static Dictionary<int, Data> dataDict;

        static async Task Main(string[] args)
        {
            dataDict = new Dictionary<int, Data>();
            dataDict.Add(0x19, new BikeInfo());
            dataDict.Add(0x10, new GeneralBikeInfo());
            dataDict.Add(0x16, new Heartrate());

            int errorCode = 0;
            BLE bleBike = new BLE();
            BLE bleHeart = new BLE();
            Thread.Sleep(1000); // We need some time to list available devices

            // List available devices
            List<String> bleBikeList = bleBike.ListDevices();
            Console.WriteLine("Devices found: ");
            foreach (var name in bleBikeList)
            {
                Console.WriteLine($"Device: {name}");
            }

            // Connecting
            errorCode = errorCode = await bleBike.OpenDevice("Tacx Flux 00472");
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
            bleBike.SubscriptionValueChanged += BleBike_SubscriptionValueChanged;
            errorCode = await bleBike.SubscribeToCharacteristic("6e40fec2-b5a3-f393-e0a9-e50e24dcca9e");


            //while (true)
            //{
            //    string input = Console.ReadLine();

            //    if (Regex.Match(input, "^[0-9]+$").Success)
            //    {
            //        await bleBike.WriteCharacteristic("669aa501-0c08-969e-e211-86ad5062675f", sendMessage(float.Parse(input)));
            //        await bleBike.WriteCharacteristic("6e40fec3-b5a3-f393-e0a9-e50e24dcca9e", sendResistanceMessage(float.Parse(input)));
            //    }

            //}

            Console.Read();
           
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

        private static void BleBike_SubscriptionValueChanged(object sender, BLESubscriptionValueChangedEventArgs e)
        {
            Console.WriteLine("Received from {0}: {1}, {2}", e.ServiceName,
                BitConverter.ToString(e.Data).Replace("-", " "),
                Encoding.UTF8.GetString(e.Data));
            Data data = null;
            if (e.ServiceName == "6e40fec2-b5a3-f393-e0a9-e50e24dcca9e")
            {
                data = dataDict[e.Data[4]];
            }
            else if (e.ServiceName == "HeartRateMeasurement")
            {
                data = dataDict[e.Data[0]];
            }


            if (data != null)
            {
                data.Update(e.Data);
            }

        }

    }
}
