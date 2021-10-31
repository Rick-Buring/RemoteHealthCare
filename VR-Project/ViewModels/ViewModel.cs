
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

namespace VR_Project.ViewModels
{
    public class ViewModel : BaseViewModel
    {
        public delegate void Update(Ergometer ergometer, HeartBeatMonitor heartBeatMonitor);
        public delegate void SendResistance(float resistance);
        public static Update updater;
        public static SendResistance resistanceUpdater;


        public BaseViewModel CurrentPageViewModel { get; private set; }

        private void ChangeViewModel(BaseViewModel viewModel)
        {
            if (CurrentPageViewModel != null)
                CurrentPageViewModel.NavigateEvent -= ChangeViewModel;

            CurrentPageViewModel = viewModel;
            CurrentPageViewModel.NavigateEvent += ChangeViewModel;
        }

        public ViewModel()
        {
            ChangeViewModel(new LoginBikeVRVM(new VrManager(), new EquipmentMain()));
        }

        public void Window_Closed(object sender, EventArgs e)
        {
            CurrentPageViewModel.Dispose();

            Debug.WriteLine("Closing and disposing client.");
        }

        public override void Dispose()
        {
            CurrentPageViewModel.Dispose();
        }
    }

}
