using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading;
using Vr_Project.RemoteHealthcare;

namespace VR_Project.ViewModels
{
    class LoginBikeVRVM : BindableBase, IDisposable
    {
        public DelegateCommand Refresh { get; }
        public ObservableCollection<Data> Engines { get; }

        public DelegateCommand SelectEngine { get; }

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

        }

        private async void GetOnlineEngines()
        {
            this.Engines.Clear();
            this.Engines.AddRange(await vr.GetEngineData());
        }

        public Data SelectClient { get; set; }
        private async void engageEngine()
        {
            if (SelectClient == null)
                return;
            
            this.equipmentThread = new Thread(async () => await this.eq.start(this.SimulationChecked));
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

        private string _BikeName;
        public string BikeName
        {
            get { return _BikeName; }
            set
            {
                if (value != _BikeName)
                {
                    _BikeName = value;
                    RaisePropertyChanged(nameof(BikeName));
                }
            }
        }
        private string _patientName;
        public string PatientName
        {
            get { return _patientName; }
            set
            {
                if (value != _patientName)
                {
                    _patientName = value;
                    RaisePropertyChanged(nameof(PatientName));
                }
            }
        }

    }
}
