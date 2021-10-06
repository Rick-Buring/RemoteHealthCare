﻿
using Newtonsoft.Json.Linq;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;
using Vr_Project.RemoteHealthcare;
using VR_Project.ViewModels;

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

        private BindableBase _currentPageViewModel;
        private List<BindableBase> _pageViewModels;

        public List<BindableBase> PageViewModels
        {
            get
            {
                if (_pageViewModels == null)
                    _pageViewModels = new List<BindableBase>();

                return _pageViewModels;
            }
        }

        public BindableBase CurrentPageViewModel
        {
            get
            {
                return _currentPageViewModel;
            }
            set
            {
                _currentPageViewModel = value;
                RaisePropertyChanged("CurrentPageViewModel");
            }
        }

        private void ChangeViewModel(BindableBase viewModel)
        {
            if (!PageViewModels.Contains(viewModel))
                PageViewModels.Add(viewModel);

            CurrentPageViewModel = PageViewModels
                .FirstOrDefault(vm => vm == viewModel);
        }

        private void onGoToConnectToServer()
        {
            ChangeViewModel(PageViewModels.Find(m => m.GetType().FullName == typeof(ConnectToServerVM).FullName));
        }

        private void OnGoToConnected()
        {
            ChangeViewModel(PageViewModels.Find(m => m.GetType().FullName == typeof(ConnectedVM).FullName));
        }
        private void OnGoToLoginBikeVR()
        {
            ChangeViewModel(PageViewModels.Find(m => m.GetType().FullName == typeof(LoginBikeVRVM).FullName));
        }

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


            PageViewModels.Add(new LoginBikeVRVM(vrManager, equipment));
            PageViewModels.Add(new ConnectToServerVM(client));
            PageViewModels.Add(new ConnectedVM(client, vrManager, equipment));
        private void EngageConnection()
        {
            this.serverConnectionThread = new Thread(client.StartConnection);
            this.serverConnectionThread.Start();
            this.resistanceUpdater += this.equipment.ergometer.SendResistance;
        }


            Mediator.Subscribe("ConnectToServer", onGoToConnectToServer);
            Mediator.Subscribe("LoginBikeVR", OnGoToLoginBikeVR);
            Mediator.Subscribe("Connected", OnGoToConnected);

            OnGoToLoginBikeVR();
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
    public static class Mediator
    {
        private static IDictionary<string, List<Action>> pl_dict =
           new Dictionary<string, List<Action>>();

        public static void Subscribe(string token, Action callback)
        {
            if (!pl_dict.ContainsKey(token))
            {
                var list = new List<Action>();
                list.Add(callback);
                pl_dict.Add(token, list);
            }
            else
            {
                bool found = false;
                foreach (var item in pl_dict[token])
                    if (item.Method.ToString() == callback.Method.ToString())
                        found = true;
                if (!found)
                    pl_dict[token].Add(callback);
            }
        }

        public static void Unsubscribe(string token, Action callback)
        {
            if (pl_dict.ContainsKey(token))
                pl_dict[token].Remove(callback);
        }

        public static void Notify(string token)
        {
            if (pl_dict.ContainsKey(token))
                foreach (var callback in pl_dict[token])
                    callback();
        }
    }

}
