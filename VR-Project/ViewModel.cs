
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using Vr_Project.RemoteHealthcare;


namespace VR_Project
{
    public class ViewModel : BindableBase
    {

        public delegate void Update(Ergometer ergometer, HeartBeatMonitor heartBeatMonitor);
        public delegate void SendResistance(float resistance);
        public Update updater;
        public SendResistance resistanceUpdater;


        private VrManager vrManager;
        private EquipmentMain equipment;
        private ClientHandler client;

        private Thread serverConnectionThread;


        public ObservableCollection<Data> Engines { get; }

        public DelegateCommand SelectEngine { get; }
        public DelegateCommand ConnectToServer { get; }



        public ViewModel()
        {
            this.SelectEngine = new DelegateCommand(engageEngine);
            this.ConnectToServer = new DelegateCommand(EngageConnection);
            this.Engines = new ObservableCollection<Data>();
            this.vrManager = new VrManager();
            this.client = new ClientHandler(this.resistanceUpdater);
            this.updater += this.vrManager.Update;
            this.updater += this.client.Update;

            this.equipment = new EquipmentMain(updater);
            GetOnlineEngines();
        }

        private async void GetOnlineEngines()
        {
            this.Engines.Clear();
            this.Engines.AddRange(await vrManager.GetEngineData());
        }

        private void EngageConnection()
        {
            this.serverConnectionThread = new Thread(client.StartConnection);
            this.serverConnectionThread.Start();
            this.resistanceUpdater += this.equipment.ergometer.SendResistance;
            this.vrManager.ResistanceUpdater = this.resistanceUpdater;
            
        }

        public Data SelectClient { get; set; }

        private async void engageEngine()
        {
            if (SelectClient == null)
                return;
            await this.equipment.start();
            await this.vrManager.ConnectToTunnel(SelectClient.id);
        }

        public void Window_Closed(object sender, EventArgs e)
        {

            client.Stop();
            if (serverConnectionThread != null)
                serverConnectionThread.Join();

            Debug.WriteLine("Closing and disposing client.");
            this.vrManager.CloseConnection();
            this.equipment.Dispose();
        }
    }
}
