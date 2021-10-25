using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Threading;
using System.Linq;
using Vr_Project.RemoteHealthcare;
using System.Threading.Tasks;
using System.Diagnostics;

namespace VR_Project.ViewModels
{
    class LoginBikeVRVM : BaseViewModel
    {
        public DelegateCommand Refresh { get; }
        public ObservableCollection<Data> Engines { get; }
        public DelegateCommand SelectEngine { get; }
        public string BikeName { get; set; } = "Tacx Flux 01249";
        public bool SimulationChecked { get; set; }

        private readonly ViewModel.NavigateViewModel navigate;
        private VrManager vr;
        private EquipmentMain eq;
        public bool isRefresheble { get; set; }
        public bool isConnecting { get; set; }
        public bool selectedAClient => SelectClient != null ;
        public Data SelectClient { get; set; }

        public LoginBikeVRVM(VrManager vr, EquipmentMain eq, ViewModel.NavigateViewModel changeViewModel)
        {
            this.vr = vr;
            this.eq = eq;
            this.navigate = changeViewModel;


            this.Refresh = new DelegateCommand(GetOnlineEngines);
            this.SelectEngine = new DelegateCommand(engageEngine);
            this.Engines = new ObservableCollection<Data>();
            GetOnlineEngines();
        }

        private async void GetOnlineEngines()
        {
            this.isRefresheble = false;
            try
            {
                this.Engines.Clear();
                this.Engines.AddRange(await vr.GetEngineData());
                this.SelectClient = this.Engines.LastOrDefault((Client) => Client.clientinfo.host == Environment.MachineName);
            }
            catch (ArgumentNullException ex)
            {
                Debug.WriteLine("Engines where null");
            }
            finally
            {
                this.isRefresheble = true;
            }
        }

        private async void engageEngine()
        {
            isConnecting = true;
            Task equipment = this.eq.start(BikeName, this.SimulationChecked);
            Task virtualReality = vr.ConnectToTunnel(SelectClient.id);
            
            Task.WaitAll(equipment,virtualReality);
            navigate(new ConnectToServerVM(new ClientHandler(), eq, vr, navigate));
            isConnecting = false;
        }

        public override void Dispose() { }

    }
}
