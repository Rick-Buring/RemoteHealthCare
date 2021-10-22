using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading;
using Vr_Project.RemoteHealthcare;

namespace VR_Project.ViewModels
{
    class ConnectToServerVM : BaseViewModel
    {
        private Thread serverConnectionThread;
        private VrManager vr;

        public ClientHandler Client { get; private set; }
        private EquipmentMain eq;
        private ViewModel.NavigateViewModel navigateView;
        public DelegateCommand ConnectToServer{ get; }

        public ConnectToServerVM(ClientHandler client, EquipmentMain equipment,VrManager vr, ViewModel.NavigateViewModel navigate)
        {
            this.vr = vr;
            this.Client = client;
            this.eq = equipment;
            this.navigateView = navigate;
            this.ConnectToServer = new DelegateCommand(EngageConnection);
        }

        public string PortNumber { get; set; } = "5005";
        public string IPAddress { get; set; } = "localHost";

        private void EngageConnection()
        {
            this.serverConnectionThread = new Thread(() => Client.StartConnection(this.IPAddress, Int32.Parse(this.PortNumber)));
            this.serverConnectionThread.Start();
            ViewModel.resistanceUpdater += this.eq.Ergometer.SendResistance;
            navigateView(new ConnectedVM(Client, vr, eq, navigateView));
        }


        public override void Dispose()
        {
            if (serverConnectionThread != null && serverConnectionThread.IsAlive)
            {
                serverConnectionThread.Abort();
            }

        }
    }
}
