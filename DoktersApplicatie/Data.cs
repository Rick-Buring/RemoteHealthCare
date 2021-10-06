using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Windows.Controls;

namespace DoktersApplicatie
{
    class Data
    {

        public List<Employee> employees { get; set; }

        public Data()
        {
            employees = new List<Employee>();
            employees.Add(new Employee { Name = "Kapil Malhotra", Age = 30 });
            employees.Add(new Employee { Name = "Raj Kundra", Age = 34 });
            employees.Add(new Employee { Name = "Amitabh Bachan", Age = 80 });
            employees.Add(new Employee { Name = "Deepak Khanna", Age = 72 });
        }


    }

    public class Employee : BindableBase, INotifyPropertyChanged
    {
        public string Name { get; set; }
        public int Age { get; set; }

        public DelegateCommand<object> MoreInfo { get; private set; }

        public DelegateCommand ExecuteDelegateCommand { get; private set; }

        public Employee()
        {

            MoreInfo = new DelegateCommand<object>(ToMoreInfo, canSubmit);

        }

        public void ToMoreInfo(object parameter)
        {
            Debug.WriteLine(parameter);
            TabControl tabControl = parameter as TabControl;
            tabControl.SelectedIndex = 1;
            Debug.WriteLine("Click");
        }

        public bool canSubmit(object parameter)
        {
            return true;
        }

    }

}
