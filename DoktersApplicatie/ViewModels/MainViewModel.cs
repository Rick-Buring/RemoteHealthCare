using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;

namespace DoktersApplicatie.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public delegate void NavigateDelegate(ViewModelBase view);

        public ViewModelBase CurrentUserControl { get; set; }

        public MainViewModel()
        {
            CurrentUserControl = new LoginVM(Navigate);
        }

        public void Navigate(ViewModelBase navigateTo)
        {
            this.CurrentUserControl = navigateTo;
        }
        public void Window_Closed(object sender, EventArgs e)
        {
            //CurrentPageViewModel.Dispose();
            CurrentUserControl.Dispose();
            Debug.WriteLine("Closing and disposing client.");
        }

		public override void Dispose()
		{

			//throw new NotImplementedException();
		}
	}
}
