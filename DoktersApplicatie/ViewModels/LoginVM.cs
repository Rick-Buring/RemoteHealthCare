using DoktersApplicatie.Commands;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using static DoktersApplicatie.ViewModels.MainViewModel;

namespace DoktersApplicatie.ViewModels
{
    public class LoginVM : ViewModelBase
    {
        public string Name { get; set; }
        public string Password { get; set; }

        public string Message { get; internal set; }
        public bool MessageIsError { get; internal set; }

        public AsyncLoginCommand<HomeVM> LoginCommand { get; }
        public string ServerAddres { get; set; } = "Localhost:6006";

        public LoginVM(NavigateDelegate navigate)
        {
            this.LoginCommand = new AsyncLoginCommand<HomeVM>(navigate, this, (ex) => Console.WriteLine(ex.Message));
        }

		public override void Dispose()
		{
            
		}
	}
}
