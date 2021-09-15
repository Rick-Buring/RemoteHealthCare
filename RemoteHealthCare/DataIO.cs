using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteHealthCare
{

    //class om data naar de database te versturen
    class DataIO : IDataListener
    {
        //incomende data wordt hier afgelevert
        public void notify(IData data)
        {
            //throw new NotImplementedException();
        }
    }
}
