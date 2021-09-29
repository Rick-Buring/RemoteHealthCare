using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VR_Project
{
    public interface EngineCallback
    {
        // __CR__ [PSMG] Waarom heb je hier voor een callback gekozen
        public abstract void notify(ObservableCollection<VrManager.Data> ob);

    }
}
