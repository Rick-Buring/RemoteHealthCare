using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace DoktersApplicatie
{
    class ViewModel : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        public List<Employee> employees;

        public ViewModel()
        {

            Data data = new Data();
            this.employees = data.employees;

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
