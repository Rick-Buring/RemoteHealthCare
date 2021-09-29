using Avans.TI.BLE;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Vr_Project.RemoteHealthcare
{
    public class Ergometer : Sensor
    {
        public string Name { get; }

        private IDataListener[] listeners;
        private BLE bleBike;
        protected ErgometerData ergometerData;

        /// <summary>
        /// constructor voor de fiets
        /// </summary>
        /// <param name="name">de naam van de fiets waarmee word verbonden</param>
        /// <param name="listener"> een lijst met classes die genotificeert willen worden op nieuwe data</param>
        public Ergometer(string name, params IDataListener[] listener)
        {
            this.ergometerData = new ErgometerData();
            this.Name = name;
            this.bleBike = new BLE();
            this.listeners = listener;

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

            int errorCode = 1;
            // Verbinden met de fiets en check op errors
            while (errorCode == 1)
            {
                errorCode = await bleBike.OpenDevice(Name);
            }

            //geef een lijst van beschikbare servies
            var services = bleBike.GetServices;
            foreach (var service in services)
            {
                Console.WriteLine($"Service: {service.Name}");
            }

            // start de service die gebruikt gaat worden
            errorCode = 1;
            while (errorCode == 1)
            {
                errorCode = await bleBike.SetService("6e40fec1-b5a3-f393-e0a9-e50e24dcca9e");
            }


            // Subscribe naar de service
            bleBike.SubscriptionValueChanged += SubscriptionValueChanged;
            errorCode = 1;
            while (errorCode == 1)
            {
                errorCode = await bleBike.SubscribeToCharacteristic("6e40fec2-b5a3-f393-e0a9-e50e24dcca9e");
            }

            //bleBike.WriteCharacteristic("6e40fec3-b5a3-f393-e0a9-e50e24dcca9e", ResistanceMessage(70));
        }

        //event voor binnenkomende data notificeren van de classes
        public override void SubscriptionValueChanged(object sender, BLESubscriptionValueChangedEventArgs e)
        {
            byte[] data = new List<byte>(e.Data).GetRange(0, e.Data.Length).ToArray();

            if (checksum(data) == e.Data[e.Data.Length - 1])
            {
                Debug.WriteLine("Checksum correct");
                this.ergometerData.Update(e.Data);
                notifyListeners();
            } else
            {
                Debug.WriteLine("Checksum incorrect");
            }
            
        }

        /// <summary>
        /// methode om weerstant bericht te maken
        /// </summary>
        /// <param name="resistance">hoeveel weerstant de fiets moet bieden in hele procenten met een max van 100</param>
        /// <returns>een bye array die naar de fiets gestuurt kan worden</returns>
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
        /// <summary>
        /// checksum berekenen
        /// </summary>
        /// <param name="message"></param>
        /// <returns> de checksum</returns>
        private static byte checksum(byte[] message)
        {
            byte output = message[0];

            for (int i = 1; i < message.Length - 1; i++)
            {
                output = (byte)(output ^ message[i]);
            }

            return output;
        }
        /// <summary>
        /// notificeren van luisteraars
        /// </summary>
        private void notifyListeners()
        {
            for (int i = 0; i < this.listeners.Length; i++)
            {
                this.listeners[i].notify(ergometerData);
            }
        }

        public override int GetHeartBeat()
        {
            return 0;
        }

        public override ErgometerData GetErgometerData()
        {
            return this.ergometerData;
        }

    }
}
