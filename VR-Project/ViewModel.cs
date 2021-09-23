﻿
using System.Collections.ObjectModel;
using System.Windows.Input;
using VR_Project.Util;


namespace VR_Project
{
    class ViewModel : ObservableObject, EngineCallback
    {

        //public delegate void Update(Ergometer ergometer, HeartBeatMonitor heartBeatMonitor);
        //public Update update;
        private VrManager vrManager;
        private EquipmentManager equipment;

        


        public ViewModel()
        {
            this.vrManager = new VrManager(this);

            //his.update = NotifyData;
            this.equipment = new EquipmentManager();
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


        private VrManager.Data _selectedMilight;

        public VrManager.Data SelectedMilight
        {
            get { return _selectedMilight; }
            set
            {
                _selectedMilight = value;
            }
        }
        private void button_Click()
        {
            //if (SelectedMilight == null)
            //    return;
            //this.tunnelID = SelectedMilight.id;
            if (SelectedMilight == null)
                return;
            this.vrManager.connectToTunnel(SelectedMilight.id);
            this.equipment.startEquipment();
            //connectToTunnel();

        }
        public ObservableCollection<VrManager.Data> ob { get; set; }
        public void notify(ObservableCollection<VrManager.Data> ob)
        {
            this.ob = ob;
        }

        //public void NotifyData (Ergometer ergometer, HeartBeatMonitor heartBeatMonitor)
        //{

        //}
    }
}
