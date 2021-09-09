using Avans.TI.BLE;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace RemoteHealthCare
{
    class Program : IDataListener
    {

        //private static string bikeData;
        //private static string heartRateData;

        //private static bool receivedBikeData;
        //private static bool receivedHeartRateData;

        //static async Task Main(string[] args)
        //{
        //    Program program = new Program();
        //    await program.start();
        //}

        //private async Task  start ()
        //{
        //    bikeData = "";
        //    heartRateData = "";
        //    receivedBikeData = false;
        //    receivedHeartRateData = false;


        //    Bike bike = new Bike(this);

        //    await bike.connect("Tacx Flux 01140", true);
        //    HeartbeatMonitor hrm = new HeartbeatMonitor(this);
        //    await hrm.connect(true);


        //    Console.Read();
        //}

        private static byte[] sendMessage(float resistance)
        {
            byte[] buff = new byte[13];
            buff[0] = 0xa4; //sync byte
            buff[1] = 0x09; //todo length
            buff[2] = 0x4e; // message type
            buff[3] = 0x05; //channel
            buff[4] = 0x30; //page

            //buff[5] = 0xff;
            //buff[6] = 0xff;
            //buff[7] = 0xff;
            //buff[8] = 0xff;
            //buff[9] = 0xff;
            //buff[10] = 0xff;

            buff[11] = (byte)(resistance * 2.0f);
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
        }

        public void notify(string data, int id)
        {
            Console.Clear();
            Console.WriteLine(data);
        }

        public void notify(IData data)
        {
            throw new NotImplementedException();
        }
    }

}
