
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using Vr_Project.RemoteHealthcare;
using VR_Project.Util;


namespace VR_Project
{
    public class ViewModel : ObservableObject, EngineCallback
    {

        public delegate void Update(Ergometer ergometer, HeartBeatMonitor heartBeatMonitor);
        public Update updater;
        private VrManager vrManager;
        private EquipmentManager equipment;




        public ViewModel()
        {
            this.vrManager = new VrManager(this);

            updater = NotifyData;
            this.equipment = new EquipmentManager(updater);
        }

        private ICommand _selectEngine;
        public ICommand SelectEngine
        {
            get
            {

                if (_selectEngine == null)
                {
                    _selectEngine = new RelayCommand(param => this.button_Click());
                }

                return _selectEngine;
            }
        }


        private VrManager.Data selectClient;

        public VrManager.Data SelectClient
        {
            get { return selectClient; }
            set
            {
                selectClient = value;
            }
        }
        private void button_Click()
        {
            //if (SelectedMilight == null)
            //    return;
            //this.tunnelID = SelectedMilight.id;
            if (SelectClient == null)
                return;
            this.vrManager.connectToTunnel(SelectClient.id);
            this.equipment.startEquipment();
            //connectToTunnel();

        }
        public ObservableCollection<VrManager.Data> ob { get; set; }
        public void notify(ObservableCollection<VrManager.Data> ob)
        {
            this.ob = ob;
        }

        public void NotifyData(Ergometer ergometer, HeartBeatMonitor heartBeatMonitor)
        {
            Debug.WriteLine("From: ViewModel");
            Debug.WriteLine($"{ergometer.GetData()}\n{heartBeatMonitor.GetData()}");
        }
    }
}
