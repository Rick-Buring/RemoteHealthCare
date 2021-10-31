using CommunicationObjects.DataObjects;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Windows;

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

        private ClientHandler clientHandler { get; set; }

        public HistoryVM(string[] clients, ClientHandler handler)
        {
            cRetrieveHistory = new DelegateCommand(RetrieveHistory);

            this.ClientHistories = new List<History>();
            this.clientHandler = handler;

            foreach (string client in clients)
            {
                this.ClientHistories.Add(new History { clientName = client });
            }

            Application.Current.Dispatcher.Invoke(() =>
            {
                var window = new HistoryWindow
                {
                    DataContext = this
                };

                window.Show();
            });
        }

        public void RetrieveHistory()
        {
            this.clientHandler.RequestHistory(SelectedClientHistory);
        }

    }
}
