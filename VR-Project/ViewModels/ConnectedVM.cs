using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Text;
using Vr_Project.RemoteHealthcare;

namespace VR_Project.ViewModels
{
    class ConnectedVM : BindableBase, IDisposable
    {
        public ConnectedVM(ClientHandler client, VrManager vrManager, EquipmentMain equipment)
        {
            Client = client;
            VrManager = vrManager;
            Equipment = equipment;
        }

        public ClientHandler Client { get; }
        public VrManager VrManager { get; }
        public EquipmentMain Equipment { get; }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
