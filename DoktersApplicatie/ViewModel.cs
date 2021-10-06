using Prism.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;
using Prism.Mvvm;

namespace DoktersApplicatie
{
    class ViewModel : BindableBase, INotifyPropertyChanged
    {

        public DelegateCommand<object> MoreInfo { get; private set; }

        public DelegateCommand ExecuteDelegateCommand { get; private set; }

        public ViewModel()
        {
            Data data = new Data();
            this.Employees = data.employees;
            //this.activeMainTab = 0;
            //this.activeSecondaryTab = 0;
            MoreInfo = new DelegateCommand<object>(ToMoreInfo, canSubmit);
        }

        public void ToMoreInfo(object parameter)
        {
            Debug.WriteLine(ActiveMainTab + " : " + ActiveSecondaryTab);
            ActiveMainTab = 1;
            ActiveSecondaryTab = 3;
            Debug.WriteLine("Click");
        }

        public bool canSubmit(object parameter)
        {
            return true;
        }

        public List<Employee> Employees
        {
            get;
            private set;
        }

        public int ActiveMainTab
        {
            get;
            set;
        }

        public int ActiveSecondaryTab
        {
            get;
            set;
        }

    }
}
