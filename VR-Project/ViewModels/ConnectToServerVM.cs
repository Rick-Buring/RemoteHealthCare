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

        public string PortNumber { get; set; }
        public string IPAddress { get; set; }

        private void EngageConnection()
        {
            this.serverConnectionThread = new Thread(() => client.StartConnection(this.IPAddress, Int32.Parse(this.IPAddress)));
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
