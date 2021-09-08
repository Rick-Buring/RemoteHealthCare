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

        static async Task Main(string[] args)
        {
            Bike bike = new Bike();
            HeartbeatMonitor hrm = new HeartbeatMonitor();
            await bike.connect("Tacx Flux 01140", true);
            await hrm.connect(true);


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
