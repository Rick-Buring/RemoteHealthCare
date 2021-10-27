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
using System.Threading;
using System.Windows.Threading;
using System.IO;
using System.Threading.Tasks;

namespace DoktersApplicatie.ViewModels
{
    public class HomeVM : ViewModelBase
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
        public delegate void ClientReceived(Client client);
        public ClientReceived clientReceived;
        public delegate void UpdateClient(Client client, HealthData healthData);
        public UpdateClient updateClient;
        public delegate void UpdateHistory(History history);
        public UpdateHistory updateHistory;

        public int TempResistance { get; set; }

        private ClientHandler clientHandler;
        private Thread clientThread;
        private Data data;

        private Dispatcher dispatcher;

        public HomeVM(MainViewModel.NavigateDelegate navigate, ClientHandler clientHandler)
        {
            this.data = new Data();
            this.dispatcher = Dispatcher.CurrentDispatcher;
            this.Clients = data.clients;
            this.Messages = data.messages;
            if (Clients.Count != 0)
            {
                SelectedClient = Clients[0];
            }

            SessionButtonText = "Start Session";

            cStartStopSession = new DelegateCommand(StartStopSession);
            cSoloEmergencyStop = new DelegateCommand(SoloEmergencyStop);
            cGlobalEmergencyStop = new DelegateCommand(GlobalEmergencyStop);
            cSendMessage = new DelegateCommand(SendSingleMessage);
            cSendAllMessage = new DelegateCommand(SendAllMessage);
            cSetResistance = new DelegateCommand(SetResistance);
            cOpenHistory = new DelegateCommand(OpenHistory);

            //SelectedClient = Clients[0];
            SessionButtonText = "Start Session";
            TempResistance = 50;

            this.clientReceived += this.data.AddClient;
            this.updateClient += this.data.UpdateClient;
            this.updateHistory += this.InsertHistory;
            this.clientHandler = clientHandler;
            this.clientHandler.addDelegates(clientReceived, updateClient, updateHistory);

            clientThread = new Thread(async () => await clientHandler.Run());
            clientThread.Start();
        }

        //public bool canSubmit(object parameter)
        //{
        //    return true;
        //}

        //TODO Send message to server
        public void StartStopSession()
        {
            if (SessionButtonText.Equals("Start Session"))
            {
                SessionButtonText = "Stop Session";
            }
            else
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
        public void SendMessage(string text, string receiver)
        {
            if (!String.IsNullOrEmpty(text))
            {

                //Use ^.*(?=(\ =>)) with regex to only get sender
                if (receiver == "All")
                {
                    Messages.Add(new Message { Sender = "Doctor => All", Text = text, Receiver = receiver });
                    Debug.WriteLine($"Text: \"{text}\" : Selected Client: {receiver}");
                }
                else
                {
                    Messages.Add(new Message { Sender = $"Doctor => {receiver}", Text = text, Receiver = receiver });
                    Debug.WriteLine($"Text: \"{text}\" : Selected Client: {receiver}");
                }

                TextToSend = "";

            }

            if (!String.IsNullOrEmpty(text))
            {
                Messages.Add(new Message { Sender = "Doctor", Text = text, Receiver = receiver });
                Debug.WriteLine($"Text: \"{text}\" : Selected Client: {receiver}");

                TextToSend = "";
            }

        }

        public void SetResistance()
        {
            if (SelectedClient != null)
            {
                Debug.WriteLine("Set resistance to: " + SelectedClient.Resistance);
                this.clientHandler.SetResistance(SelectedClient, SelectedClient.Resistance);
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

        public async void OpenHistory()
        {
            if (SelectedClient == null)
                return;
            await this.clientHandler.RequestHistory(SelectedClient);

        }



        public void InsertHistory(History history)
        {
            List<HealthData> HistoryData = GetHealthHistory(history);

            HistoryVM historyVM = new HistoryVM(HistoryData, SelectedClient);
            this.dispatcher.Invoke(() =>
            {
                var window = new HistoryWindow();

                window.DataContext = historyVM;
                window.Show();
            });
        }

        private List<HealthData> GetHealthHistory(History history)
        {
            List<HealthData> HistoryData = new List<HealthData>();

            string[] array = history.clientHistory.Split("\n");

            for (int i = 0; i < array.Length; i++)
            {
                string[] data = array[i].Split(",");
                int hb = int.Parse(data[0]);
                int rpm = int.Parse(data[1]);
                double speed = double.Parse(data[2]);
                int currWatt = int.Parse(data[3]);
                int accWatt = int.Parse(data[4]);
                int time = int.Parse(data[5]);
                int distance = 0;
                string d = data[6];
                if (d.Contains("\r"))
                {
                    int distanceLenght = d.Length - d.IndexOf("\r");
                    distance = int.Parse(d.Substring(0, distanceLenght));
                }
                else
                {
                    distance = int.Parse(d);
                }

                HealthData healthData = new HealthData()
                {
                    Heartbeat = hb,
                    RPM = rpm,
                    Speed = speed,
                    CurWatt = currWatt,
                    AccWatt = accWatt,
                    ElapsedTime = time,
                    DistanceTraveled = distance
                };
                HistoryData.Add(healthData);

            }

            return HistoryData;
        }

    }
}
