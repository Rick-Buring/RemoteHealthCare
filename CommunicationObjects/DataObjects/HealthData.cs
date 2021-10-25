using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationObjects.DataObjects
{
    public class HealthData
    {
        public int Heartbeat { get; set; }
        public int RPM { get; set; }
        public double Speed { get; set; }
        public int CurWatt { get; set; }
        public int AccWatt { get; set; }
        public int ElapsedTime { get; set; }
        public int DistanceTraveled { get; set; }

        public override string ToString()
        {
            return $"{Heartbeat},{RPM},{Speed},{CurWatt},{AccWatt},{ElapsedTime},{DistanceTraveled}";
        }
    }
}
