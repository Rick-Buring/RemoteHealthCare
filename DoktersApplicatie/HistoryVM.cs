using CommunicationObjects.DataObjects;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace DoktersApplicatie
{
    class HistoryVM : BindableBase, INotifyPropertyChanged
    {

        public List<HealthData> HealthData { get; set; }
        public HealthData LastHealthData { get; set; }
        public Client SelectedClient { get; }

        public HistoryVM(List<HealthData> healthData, Client selectedClient)
        {
            this.HealthData = healthData;
            this.LastHealthData = this.HealthData[this.HealthData.Count - 1];
            this.SelectedClient = selectedClient;
        }

    }
}
