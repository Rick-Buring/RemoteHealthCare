using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Vr_Project.RemoteHealthcare;

namespace VR_Project.ViewModels
{
    class ConnectToServerVM : BindableBase, IDisposable
    {
        private Thread serverConnectionThread;
        private ClientHandler client;
        private EquipmentMain eq;
        public DelegateCommand ConnectToServer { get; }

        public ConnectToServerVM(ClientHandler client, EquipmentMain equipment)
        {
            this.client = client;
            this.eq = equipment;
            this.ConnectToServer = new DelegateCommand(EngageConnection);
        }

        private string _portNumber;
        public string PortNumber
        {
            get { return _portNumber; }
            set
            {
                if (value != _portNumber)
                {
                    _portNumber = value;
                    RaisePropertyChanged(nameof(PortNumber));
                }
            }
        }
        private string _IPAddress;
        public string IPAddress
        {
            get { return _IPAddress; }
            set
            {
                if (value != _IPAddress)
                {
                    _IPAddress = value;
                    RaisePropertyChanged(nameof(_IPAddress));
                }
            }
        }


        private void EngageConnection()
        {
            this.serverConnectionThread = new Thread(client.StartConnection);
            this.serverConnectionThread.Start();
            ViewModel.resistanceUpdater += this.eq.ergometer.SendResistance;
            Mediator.Notify("Connected");
        }

        private bool notValid()
        {
            return IPAddress.Length > 0 && PortNumber.Length > 0;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
