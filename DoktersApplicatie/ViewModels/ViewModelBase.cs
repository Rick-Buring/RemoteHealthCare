using Prism.Mvvm;
using System;
using System.ComponentModel;

namespace DoktersApplicatie.ViewModels
{
	public abstract class ViewModelBase : BindableBase, INotifyPropertyChanged, IDisposable
	{
		public abstract void Dispose();
	}
}