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

        public List<Employee> employees;

        public int activeMainTab { get; set; }
        public int activeSecondaryTab { get; set; }

        public ViewModel()
        {
            Data data = new Data();
            this.employees = data.employees;
            //this.activeMainTab = 0;
            //this.activeSecondaryTab = 0;
            MoreInfo = new DelegateCommand<object>(ToMoreInfo, canSubmit);
        }

        public void ToMoreInfo(object parameter)
        {
            Debug.WriteLine(activeMainTab + " : " + activeSecondaryTab);
            activeMainTab = 1;
            activeSecondaryTab = 3;
            Debug.WriteLine("Click");
        }

        public bool canSubmit(object parameter)
        {
            return true;
        }

        public List<Employee> Employees
        {
            get
            {
                return employees;
            }
        }

        public int ActiveMainTab
        {
            get
            {
                return activeMainTab;
            }
            set
            {
                activeMainTab = value;
                RaisePropertyChanged("ActiveMainTab");
            }
        }

        public int ActiveSecondaryTab
        {
            get
            {
                return activeSecondaryTab;
            }
            set
            {
                activeSecondaryTab = value;
                RaisePropertyChanged("ActiveSecondaryTab");
            }
        }

    }
}
