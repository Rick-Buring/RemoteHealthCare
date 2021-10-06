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
        public event Update updater;

        private VrManager vrManager;
        private EquipmentMain equipment;
        private ClientHandler client;

        private Thread serverConnectionThread;

        private IPageViewModels _currentPageViewModel;
        private List<IPageViewModels> _pageViewModels;

        public List<IPageViewModels> PageViewModels
        {
            get
            {
                if (_pageViewModels == null)
                    _pageViewModels = new List<IPageViewModels>();

                return _pageViewModels;
            }
        }

        public IPageViewModels CurrentPageViewModel
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

        private void ChangeViewModel(IPageViewModels viewModel)
        {
            if (!PageViewModels.Contains(viewModel))
                PageViewModels.Add(viewModel);

            CurrentPageViewModel = PageViewModels
                .FirstOrDefault(vm => vm == viewModel);
        }

        private void onGoToConnectToServer(object obj)
        {
            ChangeViewModel(PageViewModels.Find(m => m.GetType().FullName == typeof(ConnectToServerVM).FullName));
        }

        private void OnGoToConnected(object obj)
        {
            ChangeViewModel(PageViewModels.Find(m => m.GetType().FullName == typeof(ConnectedVM).FullName));
        }
        private void OnGoToLoginBikeVR(object obj)
        {
            ChangeViewModel(PageViewModels.Find(m => m.GetType().FullName == typeof(LoginBikeVRVM).FullName));
        }

        public ViewModel()
        {
            this.vrManager = new VrManager();
            this.client = new ClientHandler();
            this.updater += this.vrManager.Update;
            this.updater += this.client.Update;

            this.equipment = new EquipmentMain(updater);


            PageViewModels.Add(new LoginBikeVRVM(vrManager, equipment));
            PageViewModels.Add(new ConnectToServerVM(client));
            PageViewModels.Add(new ConnectedVM(client, vrManager, equipment));


            Mediator.Subscribe("ConnectToServer", onGoToConnectToServer);
            Mediator.Subscribe("LoginBikeVR", OnGoToLoginBikeVR);
            Mediator.Subscribe("Connected", OnGoToConnected);

            OnGoToLoginBikeVR("");
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
        private static IDictionary<string, List<Action<object>>> pl_dict =
           new Dictionary<string, List<Action<object>>>();

        public static void Subscribe(string token, Action<object> callback)
        {
            if (!pl_dict.ContainsKey(token))
            {
                var list = new List<Action<object>>();
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

        public static void Unsubscribe(string token, Action<object> callback)
        {
            if (pl_dict.ContainsKey(token))
                pl_dict[token].Remove(callback);
        }

        public static void Notify(string token, object args = null)
        {
            if (pl_dict.ContainsKey(token))
                foreach (var callback in pl_dict[token])
                    callback(args);
        }
    }

}
