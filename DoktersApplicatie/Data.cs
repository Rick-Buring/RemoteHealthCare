using System;
using System.Collections.Generic;
using System.Text;

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

    public class Employee
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }

}
