using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationObjects.DataObjects
{
    public class Root
    {
        public string Type { get; set; }
        public string Sender { get; set; }
        public string Target { get; set; }
        public Object Data { get; set; }
    }
}
