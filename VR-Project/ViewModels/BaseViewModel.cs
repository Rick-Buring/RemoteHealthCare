using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace VR_Project.ViewModels
{
    public abstract class BaseViewModel : BindableBase, INotifyPropertyChanged, IDisposable
    {
        public abstract void Dispose();
    }
}
