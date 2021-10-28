using CommunicationObjects.DataObjects;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;

namespace DoktersApplicatie
{
    class HistoryVM : BindableBase, INotifyPropertyChanged
    {

        public DelegateCommand cRetrieveHistory { get; private set; }

        public List<HealthData> HealthData { get; set; }
        public HealthData LastHealthData { get; set; }

        public HistoryData HistoryData { get; set; }
        public List<History> ClientHistories { get; set; }
        public History SelectedClientHistory { get; set; }

        public ObservableCollection<HealthData> Clients { get; private set; }

        public HistoryVM(List<HealthData> healthData, Client selectedClient)
        {
            cRetrieveHistory = new DelegateCommand(RetrieveHistory);

            this.HealthData = healthData;

            this.LastHealthData = this.HealthData[this.HealthData.Count - 1];

            this.ClientHistories = new List<History>();
            this.ClientHistories.Add(new History { clientName = "Shaun" });
            this.ClientHistories.Add(new History { clientName = "Jope" });
            this.ClientHistories.Add(new History { clientName = "Will" });
            this.ClientHistories.Add(new History { clientName = "Shilling" });
            this.ClientHistories.Add(new History { clientName = "Shaquille" });

            this.HistoryData = new HistoryData(healthData);
        }

        public void RetrieveHistory()
        {

        }

    }
}
