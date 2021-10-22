using Prism.Commands;
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
    class LoginBikeVRVM : BaseViewModel
    {
        public DelegateCommand Refresh { get; }
        public ObservableCollection<Data> Engines { get; }
        public DelegateCommand SelectEngine { get; }
        private readonly ViewModel.NavigateViewModel navigate;

        public string BikeName { get; set; } = "Tacx Flux 01249";

        public bool SimulationChecked { get; set; }
        private VrManager vr;
        private Thread vrThread;
        private EquipmentMain eq;
        private Thread equipmentThread;

        public LoginBikeVRVM(VrManager vr, EquipmentMain eq, ViewModel.NavigateViewModel changeViewModel)
        {
            this.vr = vr;
            this.eq = eq;
            this.navigate = changeViewModel;

            this.Refresh = new DelegateCommand(GetOnlineEngines);
            this.SelectEngine = new DelegateCommand(engageEngine);
            this.Engines = new ObservableCollection<Data>();
            //GetOnlineEngines();
        }

        private async void GetOnlineEngines()
        {
            this.Engines.Clear();
            this.Engines.AddRange(await vr.GetEngineData());
        }

        public Data SelectClient { get; set; }
        private async void engageEngine()
        {
            //if (SelectClient == null)
            //    return;

            //this.equipmentThread = new Thread(async () => await this.eq.start(BikeName, this.SimulationChecked));
            //this.vrThread = new Thread(async () => await vr.ConnectToTunnel(SelectClient.id));
            //this.equipmentThread.Start();
            //this.vrThread.Start();

            navigate(new ConnectToServerVM(new ClientHandler(), eq, vr, navigate));
        }

        public override void Dispose() { }

    }
}
