using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;
using Prism.Mvvm;
using Prism.Commands;
using System.Collections.ObjectModel;

namespace DoktersApplicatie
{
    class ViewModel : BindableBase, INotifyPropertyChanged
    {

        public DelegateCommand cStartStopSession { get; private set; }
        public DelegateCommand cSoloEmergencyStop { get; private set; }
        public DelegateCommand cGlobalEmergencyStop { get; private set; }
        public DelegateCommand cSendMessage { get; private set; }
        public DelegateCommand cSendAllMessage { get; private set; }
        public DelegateCommand cSetResistance { get; private set; }

        public ObservableCollection<Client> Clients { get; private set; }
        public ObservableCollection<Message> Messages { get; private set; }

        public string TextToSend { get; set; }
        public Client SelectedClient { get; set; }

        public ViewModel()
        {
            Data data = new Data();

            this.Clients = data.clients;
            this.Messages = data.messages;

            cStartStopSession = new DelegateCommand(StartStopSession);
            cSoloEmergencyStop = new DelegateCommand(SoloEmergencyStop);
            cGlobalEmergencyStop = new DelegateCommand(GlobalEmergencyStop);
            cSendMessage = new DelegateCommand(SendSingleMessage);
            cSendAllMessage = new DelegateCommand(SendAllMessage);
            cSetResistance = new DelegateCommand(SetResistance);

            SelectedClient = Clients[0];

        }

        //public bool canSubmit(object parameter)
        //{
        //    return true;
        //}

        //TODO Send message to server
        public void StartStopSession()
        {
            Debug.WriteLine("Started/Stopped session");
        }

        public void EmergencyStop(List<Client> emergencyClients)
        {

            foreach (Client client in emergencyClients)
            {
                Debug.WriteLine($"Emergency Stopped {client.Name}.");
            }

        }

        //TODO Send message to server
        public void SoloEmergencyStop()
        {
            Debug.WriteLine("Solo Emergency Stop");
            List<Client> EmergencyList = new List<Client>();
            EmergencyList.Add(SelectedClient);
            EmergencyStop(EmergencyList);
        }

        public void GlobalEmergencyStop()
        {
            Debug.WriteLine("Global Emergency Stop");
            List<Client> EmergencyList = new List<Client>(Clients);
            EmergencyStop(EmergencyList);
        }

        //TODO Send message to server
        public void SetResistance()
        {
            Debug.WriteLine("Set resistance to: " + SelectedClient.Resistance);
        }

        //TODO Send message to server
        public void SendMessage(string text ,string receiver)
        {

            if(!String.IsNullOrEmpty(text))
            {
                Messages.Add(new Message { Sender = "Doctor", Text = text, Receiver = receiver });
                Debug.WriteLine($"Text: \"{text}\" : Selected Client: {receiver}");

                TextToSend = "";
            }

        }

        private void SendSingleMessage()
        {
            SendMessage(TextToSend, SelectedClient.Name);
        }

        public void SendAllMessage()
        {
            SendMessage(TextToSend, "All");
        }

    }
}
