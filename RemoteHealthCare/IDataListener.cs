using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteHealthCare
{
    interface IDataListener
    {

        public abstract void notify(string data);
        

    }
}
