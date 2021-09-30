using System;
using System.Collections.Generic;
using System.Text;

namespace Vr_Project.RemoteHealthcare
{
    public interface IData
    {
        /// <summary>
        /// Deze methode wordt gebruikt om een subscriber de nieuwe data te geven.
        /// </summary>
        /// <param name="bytes">Een byte array met data.</param>
        void Update(byte[] bytes);

    }
}
