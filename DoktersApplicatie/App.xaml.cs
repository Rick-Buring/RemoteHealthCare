using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace DoktersApplicatie
{
    public partial class App : Application
    {

        private ViewModel viewModel;

        protected override void OnStartup(StartupEventArgs e)
        {
            viewModel = new ViewModel();
            var window = new MainWindow();

            window.DataContext = viewModel;
            window.Show();
        }

    }
}
