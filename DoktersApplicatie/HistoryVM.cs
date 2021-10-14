using CommunicationObjects.DataObjects;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace DoktersApplicatie
{
    class HistoryVM : BindableBase, INotifyPropertyChanged
    {

        HealthData healthData { get; set; }

        public HistoryVM()
        {

        }
    }
}
