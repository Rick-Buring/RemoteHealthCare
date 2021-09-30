using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DoktersApplicatie
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Employee> employees;

        public MainWindow()
        {
            InitializeComponent();
            employees = new List<Employee>();
            employees.Add(new Employee {Name = "Kapil Malhotra", Age = 30 });
            employees.Add(new Employee {Name = "Raj Kundra", Age = 34 });
            employees.Add(new Employee {Name = "Amitabh Bachan", Age = 80 });
            employees.Add(new Employee {Name = "Deepak Khanna", Age = 72 });

            this.DataContext = this;
        }

        public List<Employee> Employees
        {
            get
            {
                return employees;
            }
        }

    }

    public class Employee
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }

}
