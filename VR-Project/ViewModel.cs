
using System.Collections.ObjectModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Input;

using VR_Project.Util;

namespace VR_Project
{
    class ViewModel : ObservableObject, EngineCallback
    {

        private VrManager vrManager;

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


        public ViewModel()
        {
            this.vrManager = new VrManager(this);
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
            //connectToTunnel();

        }
        public ObservableCollection<VrManager.Data> ob { get; set; }
        public void notify(ObservableCollection<VrManager.Data> ob)
        {
            this.ob = ob;
        }
    }
}
