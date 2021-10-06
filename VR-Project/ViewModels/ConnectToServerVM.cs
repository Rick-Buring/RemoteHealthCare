using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace VR_Project.ViewModels
{
    class ConnectToServerVM : BindableBase, IPageViewModels
    {
        private Thread serverConnectionThread;
        private ClientHandler client;


        public DelegateCommand ConnectToServer { get; }

        public ConnectToServerVM(ClientHandler client)
        {
            this.client = client;
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

            Mediator.Notify("Connected");
        }

        private bool notValid()
        {
            return IPAddress.Length > 0 && PortNumber.Length > 0;
        }
    }
}
