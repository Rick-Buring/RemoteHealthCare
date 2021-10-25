﻿using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using VR_Project;

namespace Vr_Project.RemoteHealthcare
{
    public class EquipmentMain : BindableBase, INotifyPropertyChanged, IDataListener, System.IDisposable
    {
        private DataIO dataIO;

        public Ergometer Ergometer { get; private set; }
        public HeartBeatMonitor HeartBeatMonitor { get; private set; }

        // starts the application
        public async Task start(string bikeName, bool simulationChecked)
        {
            dataIO = new DataIO();
            if (!simulationChecked)
                Ergometer = new Ergometer(bikeName, this, dataIO);
            else
                Ergometer = new ErgoSimulator(this);
            ViewModel.resistanceUpdater += Ergometer.SendResistance;
          
            Task ergoConnect = Ergometer.Connect();

            //heartBeatMonitor = new HeartBeatMonitor(this, dataIO);
            HeartBeatMonitor = new HBSimulator(this);
            Task heartBeatConnect = HeartBeatMonitor.Connect();

            Task.WaitAll(ergoConnect, heartBeatConnect);
        }

        /// <summary>
        /// Deze methode wordt aangeroepen als er een callback is.
        /// </summary>
        /// <param name="data">Data in de vorm van IData.</param>
        public void notify(IData data)
        {
            if (HeartBeatMonitor != null)
            {
                //Debug.WriteLine($"{ergometer.GetData()}\n{heartBeatMonitor.GetData()}");
                ViewModel.updater.Invoke(Ergometer, HeartBeatMonitor);
            }

        }

        public void Dispose()
        {
            if (Ergometer != null)
                Ergometer.Dispose();

        }
    }

}
