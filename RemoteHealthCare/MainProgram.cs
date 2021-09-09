﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RemoteHealthCare
{
    class MainProgram : IDataListener
    {
        private DataIO dataIO;

        private Ergometer ergometer;
        private HeartBeatMonitor HeartBeatMonitor;
        private GUI gui;

        static async Task Main(string[] args)
        {
            MainProgram program = new MainProgram();
            await program.start();
        }

        private async Task start()
        {
            dataIO = new DataIO();
            ergometer = new Ergometer("Tacx Flux 01140", this, dataIO);
            this.gui = new GUI();
            await ergometer.Connect();
           

            HeartBeatMonitor = new HeartBeatMonitor(this, dataIO);
            await HeartBeatMonitor.Connect();


            Console.Read();
        }

        public void notify(IData data)
        {
            gui.write($"{ergometer.GetData()}\n{HeartBeatMonitor.GetData()}");
        }
    }
    
}
