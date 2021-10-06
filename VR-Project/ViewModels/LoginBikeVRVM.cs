using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Vr_Project.RemoteHealthcare;

namespace VR_Project.ViewModels
{
    class LoginBikeVRVM : BindableBase, IDisposable
    {
        public DelegateCommand Refresh { get; }
        public ObservableCollection<Data> Engines { get; }

        public DelegateCommand SelectEngine { get; }

        private VrManager vr;
        private EquipmentMain eq;

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
            Mediator.Notify("ConnectToServer");
            //await this.eq.start();
            //this.vr.ConnectToTunnel(SelectClient.id);
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
