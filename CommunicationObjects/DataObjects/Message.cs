using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationObjects.DataObjects
{

    public class Message
    {
        public string Sender { get; set; }
        public string Receiver { get; set; }
        public string Text { get; set; }
    }

}
