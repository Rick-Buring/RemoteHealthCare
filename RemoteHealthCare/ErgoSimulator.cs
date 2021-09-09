using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RemoteHealthCare
{
    class ErgoSimulator : Ergometer
    {
        public ErgoSimulator(IDataListener listener) : base(listener)
        {
            throw new NotImplementedException();
        }

        public override Task Connect()
        {
            throw new NotImplementedException();
        }

    }
}
