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
        public bool ErgoSimChecked { get; set; }
        public bool HeartBeatSimChecked { get; set; }

        private VrManager vr;
        private EquipmentMain eq;
        public bool isRefresheble { get; set; }
        public bool isConnecting { get; set; }
        public bool selectedAClient => SelectClient != null ;
        public Data SelectClient { get; set; }

        public LoginBikeVRVM(VrManager vr, EquipmentMain eq)
        {
            this.vr = vr;
            this.eq = eq;


            this.Refresh = new DelegateCommand(GetOnlineEngines);
            this.SelectEngine = new DelegateCommand(engageEngine);
            this.Engines = new ObservableCollection<Data>();
            GetOnlineEngines();
        }

        private async void GetOnlineEngines()
        {
            this.isRefresheble = false;
            this.Engines.Clear();
            List<Data> list = await vr.GetEngineData();
            if (list != null) 
            {
            this.Engines.AddRange(list);
            this.SelectClient = this.Engines.LastOrDefault((Client) => Client.clientinfo.host == Environment.MachineName);
            }
            this.isRefresheble = true;
                
        }

        private async void engageEngine()
        {
            isConnecting = true;
            Task equipment = this.eq.start(BikeName, this.ErgoSimChecked, this.HeartBeatSimChecked);
            Task virtualReality = vr.ConnectToTunnel(SelectClient.id);

            await Task.WhenAll(equipment, virtualReality);
            RaiseOnNavigate(new ConnectToServerVM(new ClientHandler(), eq, vr));
            isConnecting = false;
        }

  
        public override void Dispose() { }

    }
}
