using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using VR_Project;
using VR_Project.ViewModels;

namespace Vr_Project.RemoteHealthcare
{
    public class EquipmentMain : BindableBase, INotifyPropertyChanged, IDataListener, System.IDisposable
    {

        public Ergometer Ergometer { get; private set; }
        public HeartBeatMonitor HeartBeatMonitor { get; private set; }

        public delegate void ErorDelegate(Exception ex);
        public event ErorDelegate OnBluetoothError;

        // starts the application
        public async Task start(string bikeName, bool ErgoSimulatorCheck, bool HeartBeatSimulatorCheck)
        {
            if (!ErgoSimulatorCheck)
                Ergometer = new Ergometer(bikeName, this);
            else
                Ergometer = new ErgoSimulator(this);
            ViewModel.resistanceUpdater += Ergometer.SendResistance;

            Task ergoConnect = Ergometer.Connect();

            if (!HeartBeatSimulatorCheck)
                HeartBeatMonitor = new HeartBeatMonitor(this);
            else
                HeartBeatMonitor = new HBSimulator(this);

            Task heartBeatConnect = HeartBeatMonitor.Connect();

            await Task.WhenAll(ergoConnect, heartBeatConnect);
            Ergometer.BikeErrorEvent += ErrorHandler;
        }

        private void ErrorHandler(Exception Error)
        {
            OnBluetoothError?.Invoke(Error);
        }

        /// <summary>
        /// Deze methode wordt aangeroepen als er een callback is.
        /// </summary>
        /// <param name="data">Data in de vorm van IData.</param>
        public void notify(IData data)
        {
            if (HeartBeatMonitor != null && ViewModel.updater != null)
            {
                //Debug.WriteLine($"{ergometer.GetData()}\n{heartBeatMonitor.GetData()}");
                ViewModel.updater.Invoke(Ergometer, HeartBeatMonitor);
            }

        }

        public void Dispose()
        {
            if (Ergometer != null)
                Ergometer.Dispose();
            if (HeartBeatMonitor != null)
                HeartBeatMonitor.Dispose();

        }
    }

}
