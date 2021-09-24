
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using System.Windows.Input;
using Vr_Project.RemoteHealthcare;
using VR_Project.Util;


namespace VR_Project
{
    public class ViewModel : ObservableObject, EngineCallback
    {

        public delegate void Update(Ergometer ergometer, HeartBeatMonitor heartBeatMonitor);
        public Update updater;

        private VrManager vrManager;
        private EquipmentManager equipment;
        private ClientHandler client;
        private Thread serverConnectionThread;




        public ViewModel()
        {
            this.vrManager = new VrManager(this);

            updater = NotifyData;
            this.equipment = new EquipmentManager(updater);
            this.client = new ClientHandler();
            
        }

        private ICommand _selectEngine;
        public ICommand SelectEngine
        {
            get
            {

                if (_selectEngine == null)
                {
                    _selectEngine = new RelayCommand(param => this.engageEngine());
                }

                return _selectEngine;
            }
        }

        private ICommand connectToServer;
        public ICommand ConnectToServer
        {
            get
            {

                if (connectToServer == null)
                {
                    connectToServer = new RelayCommand(param => this.EngageConnection());
                }

                return connectToServer;
            }
        }

        private void EngageConnection()
        {
            this.serverConnectionThread = new Thread(client.StartConnection);
            this.serverConnectionThread.Start();
        }


        private VrManager.Data selectClient;

        public VrManager.Data SelectClient
        {
            get { return selectClient; }
            set
            {
                selectClient = value;
            }
        }
        private void engageEngine()
        {
            if (SelectClient == null)
                return;
            this.vrManager.connectToTunnel(SelectClient.id);
            this.equipment.startEquipment();

        }
        public ObservableCollection<VrManager.Data> ob { get; set; }
        public void notify(ObservableCollection<VrManager.Data> ob)
        {
            this.ob = ob;
        }

        public void NotifyData(Ergometer ergometer, HeartBeatMonitor heartBeatMonitor)
        {


            Debug.WriteLine("From: ViewModel");
            Debug.WriteLine($"{ergometer.GetHeartBeat()}\n{heartBeatMonitor.GetHeartBeat()}");
            this.client.Update(ergometer, heartBeatMonitor);
            this.vrManager.WriteToPanel();
        }

        public void Window_Closed(object sender, EventArgs e)
        {

            client.Stop();
            
            serverConnectionThread.Join();
            Debug.WriteLine("Closing and disposing client.");
            this.vrManager.CloseConnection();
        }
    }
}
