using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using Vr_Project.RemoteHealthcare;

namespace VR_Project.ViewModels
{
    class LoginBikeVRVM : BindableBase, IDisposable, INotifyPropertyChanged
    {
        public DelegateCommand Refresh { get; }
        public ObservableCollection<Data> Engines { get; }
        public DelegateCommand SelectEngine { get; }

        public string BikeName { get; set; } = "Tacx Flux 01249";

        private VrManager vr;
        private EquipmentMain eq;

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
            this.Engines.AddRange(await vr.GetEngineData());
        }

        public Data SelectClient { get; set; }
        private async void engageEngine()
        {
            if (SelectClient == null)
                return;
            Mediator.Notify("ConnectToServer");
            await this.eq.start(BikeName);
            await this.vr.ConnectToTunnel(SelectClient.id);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

    }
}
