
using Newtonsoft.Json.Linq;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;
using Vr_Project.RemoteHealthcare;
using VR_Project.ViewModels;

namespace VR_Project
{
    public class ViewModel : BindableBase, INotifyPropertyChanged
    {
        public delegate void Update(Ergometer ergometer, HeartBeatMonitor heartBeatMonitor);
        public delegate void SendResistance(float resistance);
        public static Update updater;
        public static SendResistance resistanceUpdater;
        public delegate void RequestResistance (float resistance);
       
        public static RequestResistance requestResistance;

        private VrManager vrManager;
        private EquipmentMain equipment;
        private ClientHandler client;

        private IDisposable _currentPageViewModel;
        private List<IDisposable> _pageViewModels;

        public List<IDisposable> PageViewModels
        {
            get
            {
                if (_pageViewModels == null)
                    _pageViewModels = new List<IDisposable>();

                return _pageViewModels;
            }
        }

        public IDisposable CurrentPageViewModel
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

        private void ChangeViewModel(IDisposable viewModel)
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
            this.vrManager = new VrManager();
            this.client = new ClientHandler();
            this.equipment = new EquipmentMain();

            updater += this.vrManager.Update;
            updater += this.client.Update;

            PageViewModels.Add(new LoginBikeVRVM(vrManager, equipment));
            PageViewModels.Add(new ConnectToServerVM(client, equipment));
            PageViewModels.Add(new ConnectedVM(client, vrManager, equipment));


            Mediator.Subscribe("ConnectToServer", onGoToConnectToServer);
            Mediator.Subscribe("LoginBikeVR", OnGoToLoginBikeVR);
            Mediator.Subscribe("Connected", OnGoToConnected);

            OnGoToLoginBikeVR();
            //OnGoToConnected();
        }

        public void Window_Closed(object sender, EventArgs e)
        {

            client.Stop();
            for (int i = 0; i < PageViewModels.Count; i++)
            {
                PageViewModels[i].Dispose();
            }

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
