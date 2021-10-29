﻿using System;
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
        public string SessionButtonText { get; set; }
        public Client SelectedClient { get; set; }
        public delegate void ClientReceived(Client client);
        public ClientReceived clientReceived;
        public delegate void UpdateClient(string clientName, HealthData healthData);
        public UpdateClient updateClient;
        public delegate void UpdateHistory(History history);
        public UpdateHistory updateHistory;

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

            this.clientReceived += this.data.AddClient;
            this.updateClient += this.data.UpdateClient;
            this.clientHandler = clientHandler;
            this.clientHandler.addDelegates(clientReceived, updateClient, updateHistory);

            clientThread = new Thread(async () => await clientHandler.Run());
            clientThread.Start();
        }

        //TODO Send message to server
        public void StartStopSession()
        {
            if (SelectedClient != null)
            {
                this.clientHandler.StartStopSession(SelectedClient);

                if (SessionButtonText.Equals("Start Session"))
                {
                    
                    SessionButtonText = "Stop Session";
                    
                }
                else
                {
                    SessionButtonText = "Start Session";
                }
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
            if (SelectedClient == null) return;
			Debug.WriteLine("Solo Emergency Stop");
			//List<Client> EmergencyList = new List<Client>();
			//EmergencyList.Add(SelectedClient);
			//EmergencyStop(EmergencyList);

			this.clientHandler.EmergencyStop(false, SelectedClient);
        }

        public void GlobalEmergencyStop()
        {
            Debug.WriteLine("Global Emergency Stop");
            //List<Client> EmergencyList = new List<Client>(Clients);
            //EmergencyStop(EmergencyList);

            this.clientHandler.EmergencyStop(true, null);
        }
        public void SendMessage(string text, string receiver)
        {
            if (!string.IsNullOrEmpty(text))
            {

                //Use ^.*(?=(\ =>)) with regex to only get sender
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



    }
}