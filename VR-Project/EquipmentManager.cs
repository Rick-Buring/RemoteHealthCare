using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Vr_Project.RemoteHealthcare;

namespace VR_Project
{
    class EquipmentManager : System.IDisposable
    {
        public EquipmentMain equipment;

        public EquipmentManager (ViewModel.Update updater)
        {
            this.equipment = new EquipmentMain(updater);
            
        }

        public void Dispose()
        {
            this.equipment.Dispose();
        }

        public void startEquipment ()
        {
            Thread thread = new Thread(async (p) => await this.equipment.start());
            thread.Start();
            //await this.equipment.start();
        }


    }
}
