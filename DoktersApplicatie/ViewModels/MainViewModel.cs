using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace DoktersApplicatie.ViewModels
{
    class MainViewModel : ViewModelBase
    {
		public ViewModelBase CurrentUserControl { get; set; } = new LoginVM();

	}
}
