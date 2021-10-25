using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;

namespace DoktersApplicatie.ViewModels
{
    public class LoginVM : ViewModelBase
	{
		public string Name { get; set; }
		public string Password { get; set; }
		public DelegateCommand LoginCommand;
		public LoginVM()
		{
			this.LoginCommand = new DelegateCommand(()=> Debug.WriteLine($"Loggin in username:{Name} Password{Password}"));
		}

	}
}
