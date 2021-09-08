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
    }
}
