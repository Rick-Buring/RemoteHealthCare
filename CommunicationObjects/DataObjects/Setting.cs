using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationObjects.DataObjects
{
    public class Setting
    {
        public int res { get; set; }
        public bool emergencystop { get; set; }
        public SessionType sesionchange { get; set; }
    }

    public enum SessionType
    {
        START,
        STOP
    }
}
