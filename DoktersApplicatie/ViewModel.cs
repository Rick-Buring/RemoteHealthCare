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

        public List<Employee> Employees { get; private set; }
        public int ActiveSecondaryTab { get; set; }
        public int ActiveMainTab { get; set; }

        public ViewModel()
        {
            Data data = new Data();
            this.Employees = data.employees;
            MoreInfo = new DelegateCommand<object>(ToMoreInfo, canSubmit);
        }

        public void ToMoreInfo(object parameter)
        {
            Debug.WriteLine(parameter);
            ActiveMainTab = 1;

            foreach (var employee in Employees)
            {
                if (employee.Name.Equals(parameter))
                {
                    ActiveSecondaryTab = Employees.IndexOf(employee);
                }
            }

            Debug.WriteLine(ActiveMainTab + " : " + ActiveSecondaryTab);
        }

        public bool canSubmit(object parameter)
        {
            return true;
        }

       

    }
}
