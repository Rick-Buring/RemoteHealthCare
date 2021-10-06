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

        public ViewModel()
        {
            Data data = new Data();
            this.employees = data.employees;
            MoreInfo = new DelegateCommand<object>(ToMoreInfo, canSubmit);
        }

        public void ToMoreInfo(object parameter)
        {
            TabControl tabControl = parameter as TabControl;
            tabControl.SelectedIndex = 1;
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

    }
}
