﻿using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Threading;
using Vr_Project.RemoteHealthcare;

namespace VR_Project.ViewModels
{
    class LoginBikeVRVM : BindableBase, IDisposable, INotifyPropertyChanged
    {
        public DelegateCommand Refresh { get; }
        public ObservableCollection<Data> Engines { get; }
        public DelegateCommand SelectEngine { get; }

        public string BikeName { get; set; } = "Tacx Flux 01249";

        public bool SimulationChecked { get; set; }
        private VrManager vr;
        private Thread vrThread;
        private EquipmentMain eq;
        private Thread equipmentThread;

        public LoginBikeVRVM(VrManager vr, EquipmentMain equipment)
        {
            this.vr = vr;
            this.eq = equipment;
            
            this.Refresh = new DelegateCommand(GetOnlineEngines);
            this.SelectEngine = new DelegateCommand(engageEngine);
            this.Engines = new ObservableCollection<Data>();
            GetOnlineEngines();
        }

        private async void GetOnlineEngines()
        {
            this.Engines.Clear();
            List<Data> list = await vr.GetEngineData();
            if (list != null)
            this.Engines.AddRange(list);
        }

        public Data SelectClient { get; set; }
        private async void engageEngine()
        {
            if (SelectClient == null)
                return;
            
            this.equipmentThread = new Thread(async () => await this.eq.start(BikeName, this.SimulationChecked));
            this.vrThread = new Thread(async () => await vr.ConnectToTunnel(SelectClient.id));
            this.equipmentThread.Start();
            this.vrThread.Start();

		}
        //private async void engageEngine()
        //{
        //if (SelectClient == null)
        //return;
        //await this.equipment.start();
        //await this.vrManager.ConnectToTunnel(SelectClient.id);
        //this.resistanceUpdater += this.equipment.ergometer.SendResistance;
        //this.vrManager.ResistanceUpdater = this.resistanceUpdater;

        public void Dispose()
        {
            throw new NotImplementedException();
        }

    }
}
