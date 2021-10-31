using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using VR_Project.ViewModels;

namespace VR_Project
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            MainWindow window = new MainWindow();
            ViewModel VM = new ViewModel();
            window.DataContext = VM;
            window.Show();
            window.Closed += VM.Window_Closed;
        }
    }
}
