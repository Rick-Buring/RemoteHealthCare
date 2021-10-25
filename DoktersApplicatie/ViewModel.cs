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
using CommunicationObjects.DataObjects;

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
        public DelegateCommand cOpenHistory { get; private set; }

        public ObservableCollection<Client> Clients { get; private set; }
        public ObservableCollection<Message> Messages { get; private set; }

        public string TextToSend { get; set; }
        public string SessionButtonText { get; set; }
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
            cOpenHistory = new DelegateCommand(OpenHistory);

            if(Clients.Count != 0)
            {
                SelectedClient = Clients[0];
            }

            SessionButtonText = "Start Session";

        }

        //TODO Send message to server
        public void StartStopSession()
        {
            if(SessionButtonText.Equals("Start Session"))
            {
                SessionButtonText = "Stop Session";
            } else
            {
                SessionButtonText = "Start Session";
            }
            Debug.WriteLine("Started/Stopped session");
        }

        //TODO Send message to server
        public void EmergencyStop(List<Client> emergencyClients)
        {

            foreach (Client client in emergencyClients)
            {
                Debug.WriteLine($"Emergency Stopped {client.Name}.");
            }

        }

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
            SelectedClient.Resistance = SelectedClient.TempResistance;
            Debug.WriteLine("Set resistance to: " + SelectedClient.Resistance);
        }

        //TODO Send message to server
        public void SendMessage(string text ,string receiver)
        {

            if(!String.IsNullOrEmpty(text))
            {

                //Use ^.*(?=(\ =>)) with regex to only get sender
                if (receiver == "All")
                {
                    Messages.Add(new Message { Sender = "Doctor => All", Text = text, Receiver = receiver });
                    Debug.WriteLine($"Text: \"{text}\" : Selected Client: {receiver}");
                } else
                {
                    Messages.Add(new Message { Sender = $"Doctor => {receiver}", Text = text, Receiver = receiver });
                    Debug.WriteLine($"Text: \"{text}\" : Selected Client: {receiver}");
                }

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

        public void OpenHistory()
        {

            List<HealthData> HistoryData = new List<HealthData>();

            HistoryData.Add(new HealthData { Heartbeat = 52, RPM = 60, Speed = 35.5, CurWatt = 200, AccWatt = 410, ElapsedTime = 100, DistanceTraveled = 41 });
            HistoryData.Add(new HealthData { Heartbeat = 60, RPM = 57, Speed = 33.0, CurWatt = 210, AccWatt = 430, ElapsedTime = 101, DistanceTraveled = 42 });
            HistoryData.Add(new HealthData { Heartbeat = 42, RPM = 67, Speed = 37.0, CurWatt = 220, AccWatt = 420, ElapsedTime = 102, DistanceTraveled = 43 });
            HistoryData.Add(new HealthData { Heartbeat = 56, RPM = 65, Speed = 32.0, CurWatt = 230, AccWatt = 410, ElapsedTime = 103, DistanceTraveled = 44 });
            HistoryData.Add(new HealthData { Heartbeat = 54, RPM = 78, Speed = 33.0, CurWatt = 220, AccWatt = 430, ElapsedTime = 104, DistanceTraveled = 45 });

            HistoryVM historyVM = new HistoryVM(HistoryData, SelectedClient);
            var window = new HistoryWindow();

            window.DataContext = historyVM;
            window.Show();
        }

    }
}
