﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Vr_Project.RemoteHealthcare;

namespace VR_Project
{
    class EquipmentManager
    {
        public EquipmentMain equipment;

        public EquipmentManager ()
        {
            this.equipment = new EquipmentMain();
            
        }

        public void startEquipment ()
        {
            Thread thread = new Thread(async (p) => await this.equipment.start());
            thread.Start();
            //await this.equipment.start();
        }


    }
}
