
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
        public delegate void NavigateViewModel(BaseViewModel vm);
        public static Update updater;
        public static SendResistance resistanceUpdater;
        public delegate void RequestResistance (float resistance);
       
        public static RequestResistance requestResistance;

        public BaseViewModel CurrentPageViewModel { get; private set; }

        private void ChangeViewModel(BaseViewModel viewModel)
        {
            CurrentPageViewModel = viewModel;
        }

        public ViewModel()
        {
            ChangeViewModel(new LoginBikeVRVM(new VrManager(), new EquipmentMain(), ChangeViewModel));
            //ChangeViewModel(new LoginBikeVRVM(vrManager, equipment));
        }

        public void Window_Closed(object sender, EventArgs e)
        {
            CurrentPageViewModel.Dispose();

            Debug.WriteLine("Closing and disposing client.");
        }
    }

}
