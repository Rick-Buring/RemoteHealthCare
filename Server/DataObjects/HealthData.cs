﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.DataObjects
{
    public class HealthData
    {
        public int Heartbeat { get; set; }
        public int RPM { get; set; }
        public double Speed { get; set; }
        public int AccWatt { get; set; }
        public int TotWatt { get; set; }

        public override string ToString()
        {
            return $"{Heartbeat}\n{RPM}\n{Speed}\n{AccWatt}\n{TotWatt}";
        }
    }
}
