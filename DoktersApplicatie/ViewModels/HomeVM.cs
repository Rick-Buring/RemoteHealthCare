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
using System.Windows;

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
        public Client SelectedClient { get; set; }
        public delegate void ClientReceived(List<Client> clients);
        public ClientReceived clientReceived;
        public delegate void UpdateClient(string clientName, HealthData healthData);
        public UpdateClient updateClient;
        public delegate void UpdateHistory(History history);
        public UpdateHistory updateHistory;
        public delegate void RemovedClients(List<Client> list);
        public RemovedClients removeClients;

        private ClientHandler clientHandler;
        private Thread clientThread;
        private Data data;

        private Dispatcher dispatcher;

        public HomeVM(MainViewModel.NavigateDelegate navigate, ClientHandler clientHandler)
        {
            this.removeClients += RemoveClients;
            this.data = new Data(this.removeClients);
            this.dispatcher = Dispatcher.CurrentDispatcher;
            this.Clients = data.clients;
            this.Messages = data.messages;
            if (Clients.Count != 0)
            {
                SelectedClient = Clients[0];
            }

            cStartStopSession = new DelegateCommand(StartStopSession);
            cSoloEmergencyStop = new DelegateCommand(SoloEmergencyStop);
            cGlobalEmergencyStop = new DelegateCommand(GlobalEmergencyStop);
            cSendMessage = new DelegateCommand(SendSingleMessage);
            cSendAllMessage = new DelegateCommand(SendAllMessage);
            cSetResistance = new DelegateCommand(SetResistance);
            cOpenHistory = new DelegateCommand(OpenHistory);

            this.clientReceived += this.data.AddClients;
            this.updateClient += this.data.UpdateClient;
            this.clientHandler = clientHandler;
            this.clientHandler.addDelegates(clientReceived, updateClient, updateHistory);

            clientThread = new Thread(async () => await clientHandler.Run());
            clientThread.Start();
        }

        public void StartStopSession()
        {
            if (SelectedClient != null)
            {
                this.clientHandler.StartStopSession(SelectedClient);
                this.SelectedClient.StartStopSession();
                Debug.WriteLine("Started/Stopped session");

            }

        }

        public void RemoveClients(List<Client> list)
		{
            if (list.Contains(SelectedClient)) SelectedClient = null;
		}

        public void SoloEmergencyStop()
        {
            if (SelectedClient == null) return;
			Debug.WriteLine("Solo Emergency Stop");

			this.clientHandler.EmergencyStop(false, SelectedClient);
            
        }

        public void GlobalEmergencyStop()
        {
            Debug.WriteLine("Global Emergency Stop");

            this.clientHandler.EmergencyStop(true, null);
            SendMessage("EMERGENCY STOP, STOP NOW", "All");
        }

        public void SendMessage(string text, string receiver)
        {
            if (!string.IsNullOrEmpty(text))
            {

                if (receiver == "All")
                {
                    Messages.Add(new Message { Sender = "Doctor => All", Text = text, Receiver = receiver });
                    this.clientHandler.SendChat(true, text);
                    Debug.WriteLine($"Text: \"{text}\" : Selected Client: {receiver}");
                }
                else
                {
                    Messages.Add(new Message { Sender = $"Doctor => {receiver}", Text = text, Receiver = receiver });
                    this.clientHandler.SendChat(false, text, SelectedClient);
                    Debug.WriteLine($"Text: \"{text}\" : Selected Client: {receiver}");
                }

                TextToSend = "";

            }

        }

        public void SetResistance()
        {
            if (SelectedClient != null)
            {
                SelectedClient.Resistance = SelectedClient.TempResistance;
                Debug.WriteLine("Set resistance to: " + SelectedClient.Resistance);
                this.clientHandler.SetResistance(SelectedClient, SelectedClient.Resistance);
            }
        }

        private void SendSingleMessage()
        {
            if (SelectedClient != null)
            SendMessage(TextToSend, SelectedClient.Name);
        }

        public void SendAllMessage()
        {
            SendMessage(TextToSend, "All");
        }

        public async void OpenHistory()
        {
            await this.clientHandler.RequestClientsHistory();
        }

		public override void Dispose()
		{
            this.clientHandler.Dispose();
			//throw new NotImplementedException();
		}
	}
}
