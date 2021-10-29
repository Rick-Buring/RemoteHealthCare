using CommunicationObjects.DataObjects;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Windows.Threading;
using Vr_Project.RemoteHealthcare;

namespace VR_Project.ViewModels
{
    public class ConnectedVM : BaseViewModel
    {
        public delegate void addMessageDelegate(Message m);
        public static addMessageDelegate AddMessage;

        public DelegateCommand DisconnectCommand { get; set; }

        private ViewModel.NavigateViewModel navigate;

        public ObservableCollection<Message> Messages { get; private set; }
        private Dispatcher dispatcher;

        public static RequestResistance requestResistance;
        public delegate void RequestResistance(float resistance);
        public delegate void SendChatMessage(string message);
        public static SendChatMessage sendChat;

        public ConnectedVM(ClientHandler client, VrManager vrManager, EquipmentMain equipment, ViewModel.NavigateViewModel navigateView)
        {
            this.dispatcher = Dispatcher.CurrentDispatcher;
            this.Messages = new ObservableCollection<Message>();
            this.DisconnectCommand = new DelegateCommand(Disconnect);
            this.navigate = navigateView;
            AddMessage = addMessage;
            Client = client;
            VrManager = vrManager;
            this.Client.resistanceUpdater += VrManager.RequestResistance;
            this.Client.sendChat += VrManager.SetChatMessage;
            Equipment = equipment;
        }

        private void Disconnect()
        {
            Console.WriteLine("TODOOO Disconnect client and bike");
            navigate(new LoginBikeVRVM(new VrManager(), new EquipmentMain(), navigate));
        }

        public ClientHandler Client { get; }
        public VrManager VrManager { get; }
        public EquipmentMain Equipment { get; }

        public void addMessage(Message m)
        {
            this.dispatcher.Invoke(() => this.Messages.Add(m));
        }

        public override void Dispose()
        {
            this.Client.Dispose();
            this.Equipment.Dispose();
            this.VrManager.Dispose();

            ViewModel.resistanceUpdater = null;
            ViewModel.updater = null;
        }
    }
}
