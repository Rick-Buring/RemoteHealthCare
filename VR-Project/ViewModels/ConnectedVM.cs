using CommunicationObjects.DataObjects;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using Vr_Project.RemoteHealthcare;

namespace VR_Project.ViewModels
{
    public class ConnectedVM : BindableBase, IDisposable, INotifyPropertyChanged
    {
        public delegate void addMessageDelegate(Message m);
        public static addMessageDelegate AddMessage;

        public DelegateCommand DisconnectCommand{ get; set; }
        public ObservableCollection<Message> Messages { get; private set; }
        public ConnectedVM(ClientHandler client, VrManager vrManager, EquipmentMain equipment)
        {
            this.Messages = new ObservableCollection<Message>();
            this.DisconnectCommand = new DelegateCommand(Disconnect);
            AddMessage = addMessage;
            Client = client;
            VrManager = vrManager;
            Equipment = equipment;
        }

        private void Disconnect()
        {
            Console.WriteLine("TODOOO Disconnect client and bike");
            Mediator.Notify("LoginBikeVR");
        }

        public ClientHandler Client { get; }
        public VrManager VrManager { get; }
        public EquipmentMain Equipment { get; }

        public void addMessage(Message m)
        {
            this.Messages.Add(m);
        }

        public void Dispose() { }
    }
}
