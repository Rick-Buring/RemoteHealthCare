using System;
using System.Collections.Generic;
using System.Text;

namespace Vr_Project.RemoteHealthcare
{
    public interface IDataListener
    {
        /// <summary>
        /// Deze methode wordt gebruikt om een subscriber te notifyen.
        /// </summary>
        /// <param name="data">Data in de vorm van IData.</param>
        public abstract void notify(IData data);
        

    }
}
