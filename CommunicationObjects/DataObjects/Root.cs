using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationObjects.DataObjects
{
    public class Root
    {
        public string type { get; set; }
        public string sender { get; set; }
        public string target { get; set; }
        public Object data { get; set; }
    }
}
