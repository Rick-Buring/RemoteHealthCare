using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VR_Project
{
    interface EngineCallback
    {

        public abstract void notify(ObservableCollection<VrManager.Data> ob);

    }
}
